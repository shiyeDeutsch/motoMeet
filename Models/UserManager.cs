namespace motoMeet
{
    public class UserManager
    {
        private readonly IUserService _userService;

        public UserManager(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Person> CreateUser(Person user)
        {
            // Validate if a user with the same NationalCode already exists.
            bool exists = await _userService.ExistValidation(u => u.NationalCode == user.NationalCode);

            if (exists)
            {
                throw new Exception("User with the same NationalCode already exists.");
            }
            else
            {
                return await _userService.CreateUser(user);
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