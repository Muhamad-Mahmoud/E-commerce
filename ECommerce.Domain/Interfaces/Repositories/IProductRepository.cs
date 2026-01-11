using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository for Product operations.
    /// </summary>
    public interface IProductRepository : IRepository<Product>
    {
        // Product-specific query methods
        Task<Product?> GetWithVariantsAsync(int id, CancellationToken cancellationToken = default);
        Task<Product?> GetWithFullDetailsAsync(int id, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<Product>> GetPublishedProductsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    }
}
