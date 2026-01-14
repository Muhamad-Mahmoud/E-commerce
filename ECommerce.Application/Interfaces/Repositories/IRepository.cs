using System.Linq.Expressions;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Generic repository interface for common CRUD operations.
    /// </summary>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Gets an entity by its ID with optional related entities.
        /// </summary>
        Task<T?> GetByIdAsync(
             int id,
             params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Gets all entities with optional related entities.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Finds entities that match the specified predicate with optional related entities.
        /// </summary>
        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Gets the first entity that matches the specified predicate with optional related entities.
        /// </summary>
        Task<T?> GetFirstAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds multiple new entities.
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Updates multiple existing entities.
        /// </summary>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Deletes multiple entities.
        /// </summary>
        void DeleteRange(IEnumerable<T> entities);

        /// <summary>
        /// Checks if an entity with the specified ID exists.
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Gets the total count of entities.
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Gets the count of entities that match the specified predicate.
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}
