using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace motoMeet
{
    public interface IAuthService
    {
        Task<AuthenticationResult> AuthenticateAsync(string email, string password);
        string CreatePasswordHash(string password);
        string GenerateJwtToken(Person user);
        public string GenerateVerificationLink(int userId);
        bool CheckPassword(Person user, string password);
        Task<bool> ValidateToken(string token, int userId);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;

        }//
        // public async Task<string> AuthenticateAsync(string email, string password)
        // {
        //     // Get the user by email
        //     var user = await _userService.FindFirstByExpression(u => u.Email == email);

        //     // Check if user exists
        //     if (user == null)
        //     {
        //         throw new Exception("User with the provided email doesn't exist.");
        //     }

        //     // Check if the hashed version of the input password matches the stored hashed password
        //     if (!CheckPassword(user, password))
        //     {
        //         throw new Exception("Incorrect password.");
        //     }

        //     // Authentication successful, generate jwt token
        //     var token = GenerateJwtToken(user);

        //     return token;
        // }
        public async Task<AuthenticationResult> AuthenticateAsync(string email, string password)
        {
            // Get the user by email
            var user = await _userService.FindFirstByExpression(u => u.Email == email);

            // Check if user exists
            if (user == null)
            {
                return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Authentication failed." };
            }

            // Check if the hashed version of the input password matches the stored hashed password
            if (!CheckPassword(user, password))
            {
                return new AuthenticationResult { IsSuccess = false, ErrorMessage = "Authentication failed." };
            }

            // Authentication successful, generate jwt token
            var token = GenerateJwtToken(user);

            return new AuthenticationResult { IsSuccess = true, Token = token };
        }


        public bool CheckPassword(Person user, string password)
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

        public string CreatePasswordHash(string password)
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

        public string GenerateJwtToken(Person user)
        {
            // var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JwtSettings:SecretKey"));

            // var tokenDescriptor = new SecurityTokenDescriptor
            // {
            //     Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.ID.ToString()) }),
            //     Expires = DateTime.UtcNow.AddDays(7), // Token expiry period
            //     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            // };

            // var token = tokenHandler.CreateToken(tokenDescriptor);
            // return tokenHandler.WriteToken(token);


            var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:SecretKey"));

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);


        }
        public string GenerateVerificationLink(int userId)
        {
            // Generate a secure token (e.g., GUID, JWT, etc.)
            var token = GenerateSecureToken();

            // Construct the verification link
            // Assume "baseVerificationUrl" is the base URL for your verification endpoint
            string baseVerificationUrl = "https://localhost:7004/api/Auth/Verify";
            string verificationLink = $"{baseVerificationUrl}?token={token}&userId={userId}";

            // Save the token with the user's information in your database with an expiration time
            // You might also want to include other relevant information
            SaveVerificationToken(userId, token);

            return verificationLink;
        }

        private string GenerateSecureToken()
        {
            // Example: using a GUID as a simple token
            return Guid.NewGuid().ToString();
        }

        private async Task SaveVerificationToken(int userId, string token)
        {
            // Find the user by ID
            var user = await _userService.GetUser(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Set the verification token and its expiration
            user.VerificationToken = token;
            user.VerificationTokenExpiration = DateTime.UtcNow.AddDays(7); // Set expiration to 7 days from now

            // Update the user in the database
            await _userService.UpdateUser(user);
        }
        public async Task<bool> ValidateToken(string token, int userId)
        {
            var user = await _userService.GetUser(userId);
            if (user != null && user.VerificationToken == token && user.VerificationTokenExpiration > DateTime.UtcNow)
            {
                user.IsVerified = true;
                await _userService.UpdateUser(user);
                return true;
            }
            else return false;
        }


    }

    public class AuthenticationResult : OperationResult<T> 
    {
        public string Token { get; set; }
    }

}