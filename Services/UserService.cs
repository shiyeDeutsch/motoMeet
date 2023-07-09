using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace motoMeet
{
    public interface IUserService
    {
        Task<IEnumerable<Person>> GetUsers();
        Task<Person> GetUser(int id);
        Task<Person> CreateUser(Person person);
        Task<Person> UpdateUser(Person user);
        Task<bool> DeleteUser(int id);
        Task<bool> ExistValidation(Expression<Func<Person, bool>> expression);
        Task<Person> FindFirstByExpression(Expression<Func<Person, bool>> expression);

    }

    public class UserService : IUserService
    {
        private readonly IRepository<Person> _personRepository;


        public UserService(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<Person>> GetUsers()
        {
            return await _personRepository.GetAllAsync();
        }

        public async Task<Person> GetUser(int id)
        {
            return await _personRepository.GetByIdAsync(id);
        }

        public async Task<Person> CreateUser(Person user)
        {
            await _personRepository.AddAsync(user);
            await _personRepository.SaveAsync();
            await _personRepository.ReloadAsync(user);

            return user;
        }

        public async Task<Person> UpdateUser(Person user)
        {

            _personRepository.Update(user);
            await _personRepository.SaveAsync();
            await _personRepository.ReloadAsync(user);

            return user;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await GetUser(id);

            // Validation
            if (user == null)
            {
                return false;
            }

            try
            {
                _personRepository.Delete(user);
                await _personRepository.SaveAsync();
                return true;
            }
            catch
            {
                // In case of any errors during deletion or saving changes, return false.
                return false;
            }
        }

        public async Task<bool> ExistValidation(Expression<Func<Person, bool>> expression)
        {
            var user = await _personRepository.FindFirstByExpressionAsync(expression);
            return user != null;
        }
        public async Task<Person> FindFirstByExpression(Expression<Func<Person, bool>> expression)
        {
            var user = await _personRepository.FindFirstByExpressionAsync(expression);
            return user;
        }



    }
}