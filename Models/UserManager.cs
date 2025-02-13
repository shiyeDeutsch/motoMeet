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
                    AddedOn = DateTime.UtcNow,
                    IsVerified = false,
                    CountryId = user.CountryId,
                };
                var newUser = await _userService.CreateUser(person);
                var token = _authService.GenerateJwtToken(newUser);

                var userDto = new UserDto
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    Bio = newUser.Bio,
                    Address = newUser.Address,
                    Token = token,
                    CountryId = newUser.CountryId
                };


                return new UserCreationResult { IsSuccess = true, User = userDto, Token = token };
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
            user.EditOn = DateTime.UtcNow;
            return await _userService.UpdateUser(user);
        }

       /// <summary>
    /// Loads the Person (with navigation properties) and maps to a full UserDto.
    /// </summary>
    public async Task<UserDto> GetFullUserData(string id)
    {
        // Ideally, ensure that your _userService includes the necessary related data.
        var person = await _userService.FindFirstByExpression(p => p.id == id);
        if (person == null)
        {
            throw new Exception("User not found.");
        }

        // Map the Person entity to UserDto (including nested routes, groups, etc.)
        var userDto = new UserDto(person);

        // Optionally, if you need to add events the user is participating in,
        // load them from your EventManager and set userDto.ParticipatedEvents here.
        // Example:
        // var events = await _eventManager.GetEventsForUser(person.Id);
        // userDto.ParticipatedEvents = events.Select(e => new EventDto(e)).ToList();
        public async Task<bool> UserExists(int userId)
        {
            return await _userService.ExistValidation(u => u.Id == userId);
        }
        return userDto;
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
            string passwordRegex = @"^(?=.*\d)(?=.*[a-zA-Z]).{6,}$";
            return Regex.IsMatch(password, passwordRegex);
        }

    }
    public class UserCreationResult : OperationResult
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
}