using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository for Product operations.
    /// </summary>
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetWithVariantsAsync(int id);
        Task<Product?> GetWithFullDetailsAsync(int id);
        
        /// <summary>
        /// Searches products with filtering (price, category, search term) and pagination.
        /// </summary>
        Task<PagedResult<ProductDto>> SearchProductsAsync(ProductParams productParams);
        
        Task<IEnumerable<Product>> GetPublishedProductsAsync();
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    }
}
