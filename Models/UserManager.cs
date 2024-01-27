using System.Text.RegularExpressions;

namespace motoMeet
{
    public class UserManager
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;


        public UserManager(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }
        // public async Task<UserCreationResult> CreateUser(RegistrateModel user)
        // {
        //     // Validate if a user with the same Email already exists.
        //     bool exists = await _userService.ExistValidation(u => u.Email == user.Email);

        //     if (exists)
        //     {
        //         return new UserCreationResult { IsSuccess = false, ErrorMessage = "User with the same Email already exists." };
        //     }
        //     else
        //     {

        //         var PasswordHash = _authService.CreatePasswordHash(user.Password);
        //         var person = new Person
        //         {
        //             Email = user.Email,
        //             FirstName = user.FirstName,
        //             LastName = user.LastName,
        //             Username = user.Username,
        //             PhoneNumber = user.PhoneNumber,
        //             Bio = user.Bio,
        //             Address = user.Address,
        //             PasswordHash = PasswordHash,
        //             AddedOn = DateTime.Now,

        //         };
        //         var newUser = await _userService.CreateUser(person);
        //         var token = _authService.GenerateJwtToken(user);

        //         return new UserCreationResult { IsSuccess = true, User = user, Token = token };
        //     }
        // }


        // public async Task<(Person, string)> createUser(Person user)
        // {
        //     // Validate if a user with the same NationalCode already exists.
        //     bool exists = await _userService.ExistValidation(u => u.Email == user.Email);

        //     if (exists)
        //     {
        //         throw new Exception("User with the same Email already exists.");
        //     }
        //     else
        //     {
        //         var PasswordHashed = _AuthService.CreatePasswordHash(user.PasswordHash);
        //         user.PasswordHash = PasswordHashed;

        //         user = await _userService.CreateUser(user);
        //         var token = _AuthService.GenerateJwtToken(user);
        //         Console.WriteLine(token);
        //         return (user, token);
        //     }
        // }

        public async Task<UserCreationResult> CreateUser(RegistrateModel user)
        {


            // Validate Email Format
            if (!validateEmail(user.Email))
            {
                return new UserCreationResult { IsSuccess = false, ErrorMessage = "Invalid email format." };
            }

            // Validate Password Format
            if (!validatePassword(user.Password))
            {
                return new UserCreationResult { IsSuccess = false, ErrorMessage = "Invalid password format." };
            }

            // Validate if a user with the same Email already exists.
            bool exists = await _userService.ExistValidation(u => u.Email == user.Email);

            if (exists)
            {
                return new UserCreationResult { IsSuccess = false, ErrorMessage = "User with the same Email already exists." };
            }
            else
            {
                
                var PasswordHash = _authService.CreatePasswordHash(user.Password);
                var person = new Person
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    PhoneNumber = user.PhoneNumber,
                    Bio = user.Bio,
                    Address = user.Address,
                    PasswordHash = PasswordHash,
                    AddedOn = DateTime.Now,
                    isVerified=false,
                };
                var newUser = await _userService.CreateUser(person);
                var token = _authService.GenerateJwtToken(newUser);

                return new UserCreationResult { IsSuccess = true, User = newUser, Token = token };
            }
        }


        public async Task<IEnumerable<Person>> GetUsers()
        {
            return await _userService.GetUsers();
        }
        public async Task<Person> GetUser(int id)
        {
            return await _userService.GetUser(id);
        }

        public async Task<Person?> UpdateUser(int id, Person updatedUser)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
            {
                return null;
            }
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            // user.RoleID = updatedUser.RoleID;
            user.EditOn = DateTime.Now;
            return await _userService.UpdateUser(user);
        }

        public async Task<Person> GetUserData(string email)
        {
            return await _userService.FindFirstByExpression(p => p.Email == email);

        }


        private bool validateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Simple regular expression for email validation
            string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailRegex);
        }

        private bool validatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Example password policy: at least 8 characters, 1 number, 1 uppercase, 1 lowercase
            string passwordRegex = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}$";
            return Regex.IsMatch(password, passwordRegex);
        }

    }
    public class UserCreationResult : OperationResult
    {
        public Person User { get; set; }
        public string Token { get; set; }
    }
}