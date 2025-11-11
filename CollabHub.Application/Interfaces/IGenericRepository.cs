using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Infrastructure.Repositories.EF
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(object id);
        Task<T>GetOneAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>>GetByConditionAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> QueryByCondition(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task <bool>UpdateAsync( T entity);
        Task<bool> DeleteAsync(T entity);
        Task <int> SaveAsync();

    }
}
