using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace motoMeet
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(string email, string password);
        // Include other relevant methods, like Register, ForgotPassword etc.
    }

    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public AuthService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        public async Task<string> AuthenticateAsync(string email, string password)
        {
            // Get the user by email
            var user = await _userService.FindFirstByExpression(u => u.Email == email);

            // Check if user exists
            if (user == null)
            {
                throw new Exception("User with the provided email doesn't exist.");
            }

            // Check if the hashed version of the input password matches the stored hashed password
            if (!CheckPassword(user, password))
            {
                throw new Exception("Incorrect password.");
            }

            // Authentication successful, generate jwt token
            var token = GenerateJwtToken(user);

            return token;
        }

        private bool CheckPassword(Person user, string password)
        {
            byte[] hashBytes = Convert.FromBase64String(user.PasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private string CreatePasswordHash(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private string GenerateJwtToken(Person user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JwtSecretKey"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.ID.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiry period
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}