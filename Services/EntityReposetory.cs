using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace motoMeet
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> Find(ISpecification<T> specification);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindByExpressionAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
        Task ReloadAsync(T entity);
        // Task DeleteByIdAsync(int id);
        //Task BulkInsertAsync(IEnumerable<T> entities);
        Task<T> FindFirstByExpressionAsync(Expression<Func<T, bool>> expression);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids);
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

        public async Task<IEnumerable<T>> Find(ISpecification<T> specification)
        {
            var queryable = _context.Set<T>().AsQueryable();

            if (specification.ToExpression() != null)
            {
                queryable = queryable.Where(specification.ToExpression());
            }

            foreach (var includeExpression in specification.Includes)
            {
                queryable = queryable.Include(includeExpression);
            }

            return await queryable.ToListAsync();
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

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }
        public async Task ReloadAsync(T entity)
        {
            await _dbSet.Entry(entity).ReloadAsync();
        }

        public async Task<T> FindFirstByExpressionAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }

        public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids)
        {
            // Ensure the TEntity has an 'Id' property of type int
            var entityType = typeof(T);
            var idProperty = entityType.GetProperty("Id");
            if (idProperty == null || idProperty.PropertyType != typeof(int))
            {
                throw new InvalidOperationException($"Entity {entityType.Name} does not have an 'Id' property of type int.");
            }

            // Dynamically build the lambda expression e => ids.Contains(e.Id)
            var parameter = Expression.Parameter(entityType, "e");
            var propertyAccess = Expression.MakeMemberAccess(parameter, idProperty);
            var constant = Expression.Constant(ids);
            var containsMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(int));
            var containsExpression = Expression.Call(containsMethod, constant, propertyAccess);

            var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);

            // Use the dynamically built lambda expression in the query
            return await _dbSet.Where(lambda).ToListAsync();
        }


        private TProperty GetPropertyValue<T, TProperty>(T entity, string propertyName)
        {
            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException($"Entity of type {entityType.Name} does not have a property named '{propertyName}'.");
            }
            var value = propertyInfo.GetValue(entity);
            if (value is TProperty typedValue)
            {
                return typedValue;
            }
            throw new InvalidOperationException($"The property '{propertyName}' on entity type {entityType.Name} is not of type {typeof(TProperty).Name}.");
        }





    }
    public interface IOperationResult<T>
    {
        bool IsSuccess { get; set; }
        string ErrorMessage { get; set; }
        T Result { get; set; }
    }

    public class OperationResult<T> : IOperationResult<T>
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public T Result { get; set; }
    }

}