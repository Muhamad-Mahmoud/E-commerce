using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Product operations.
    /// </summary>
    public interface IProductRepository : IRepository<Product>
    {
        /// <summary>
        /// Gets a product by ID with its variants.
        /// </summary>
        Task<Product?> GetWithVariantsAsync(int id);

        /// <summary>
        /// Gets a product by ID with all related details including category, variants, images, and reviews.
        /// </summary>
        Task<Product?> GetWithFullDetailsAsync(int id);

        /// <summary>
        /// Searches products with filtering (price, category, search term) and pagination.
        /// </summary>
        Task<PagedResult<ProductDto>> SearchProductsAsync(ProductParams productParams);

        /// <summary>
        /// Gets all published products with their variants.
        /// </summary>
        Task<IEnumerable<Product>> GetPublishedProductsAsync();

        /// <summary>
        /// Gets all products in a specific category with their variants.
        /// </summary>
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    }
}
