using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository for Category operations.
    /// </summary>
    public interface ICategoryRepository
    {
        // Query by business needs
        Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        
        // Commands
        Task AddAsync(Category category, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
        
        // Persistence
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
