using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository for Product operations.
    /// </summary>
    public interface IProductRepository
    {
        // Query by business needs
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Product?> GetWithVariantsAsync(int id, CancellationToken cancellationToken = default);
        Task<Product?> GetWithFullDetailsAsync(int id, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<Product>> GetPublishedProductsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
        
        // Commands
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        
        // Persistence
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
