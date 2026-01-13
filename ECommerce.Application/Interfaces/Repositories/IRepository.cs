using System.Linq.Expressions;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Generic repository interface for common CRUD operations.
    /// </summary>
    public interface IRepository<T> where T : BaseEntity
    {
        //  Query Operations (Read-only) 

        Task<T?> GetByIdAsync(
             int id,
             params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetAllAsync(
            params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        Task<T?> GetFirstAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        //  Command Operations 

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        //  Utility 

        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}
