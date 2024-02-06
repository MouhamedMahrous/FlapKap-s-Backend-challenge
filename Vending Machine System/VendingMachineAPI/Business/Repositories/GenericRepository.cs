
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;

namespace VendingMachineAPI.Business.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected MachineDBContext _context;
        internal DbSet<T> _dbSet;
        protected readonly ILogger _logger;

        public GenericRepository(MachineDBContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<T>();
        }
        public virtual async Task<IEnumerable<T>> AllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return true;
        }

        public virtual bool Delete(T entity)
        {
            _dbSet.Remove(entity);
            return true;
        }

        public virtual bool Update(T entity)
        {
            _dbSet.Update(entity);
            return true;
        }
    }
}
