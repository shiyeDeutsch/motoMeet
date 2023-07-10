namespace motoMeet
{
    public class UserManager
    {
        private readonly IUserService _userService;
        private readonly IAuthService _AuthService;


        public UserManager(IUserService userService, IAuthService AuthService)
        {
            _userService = userService;
            _AuthService = AuthService;
        }

        // public async Task<Person> CreateUser(Person user)
        // {
        //     // Validate if a user with the same NationalCode already exists.
        //     bool exists = await _userService.ExistValidation(u => u.NationalCode == user.NationalCode);

        //     if (exists)
        //     {
        //         throw new Exception("User with the same NationalCode already exists.");
        //     }
        //     else
        //     {
        //         var PasswordHashed = _AuthService.CreatePasswordHash(user.Email);
        //         user.Email = PasswordHashed;

        //         user = await _userService.CreateUser(user);
        //         var token = _AuthService.GenerateJwtToken(user);
        //         return user;
        //     }
        // }

        public async Task<(Person, string)> CreateUser(Person user)
        {
            // Validate if a user with the same NationalCode already exists.
            bool exists = await _userService.ExistValidation(u => u.NationalCode == user.NationalCode);

            if (exists)
            {
                throw new Exception("User with the same NationalCode already exists.");
            }
            else
            {
                var PasswordHashed = _AuthService.CreatePasswordHash(user.Email);
                user.Email = PasswordHashed;

                user = await _userService.CreateUser(user);
                var token = _AuthService.GenerateJwtToken(user);
                return (user, token);
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
            user.RoleID = updatedUser.RoleID;
            user.EditOn = DateTime.Now;





            return await _userService.UpdateUser(user);
        }

    }
}