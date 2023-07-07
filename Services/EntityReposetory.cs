using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace motoMeet
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindByExpressionAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
        Task ReloadAsync(T entity);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MotoMeetDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(MotoMeetDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindByExpressionAsync(Expression<Func<T, bool>> expression)
        {

            return await _dbSet.Where(expression).ToListAsync();

        }


        public async Task ReloadAsync(T entity)
        {
            await _dbSet.Entry(entity).ReloadAsync();
        }

    }

}