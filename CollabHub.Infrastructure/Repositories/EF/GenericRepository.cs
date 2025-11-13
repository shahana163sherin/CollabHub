using CollabHub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Infrastructure.Repositories.EF
{
    public class GenericRepository <T>:IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }
        public async Task <IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T> GetByIdAsync(object id)=>await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>>GetByConditionAsync(Expression<Func<T,bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return await _dbSet.Where(predicate).ToListAsync();
        }
        public IQueryable<T> QueryByCondition(Expression<Func<T, bool>> predicate)
        {
            if(predicate == null)throw new ArgumentNullException(nameof(predicate));

            return _dbSet.Where(predicate);
        }

        public async Task<T?> GetOneAsync(Expression<Func<T,bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        public async Task AddAsync(T entity)
        {
            if(entity == null)
                throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
        }
               

        public async Task<bool> DeleteAsync(T entity)
        {
            if (entity == null)
            {
                return false;
            }
            _dbSet.Remove(entity);
             return true;
        }
       public async Task <int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }


        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null){
                return false;
            }
                     
            
           _dbSet.Update(entity);
            return true;
        }

       
    }
}
