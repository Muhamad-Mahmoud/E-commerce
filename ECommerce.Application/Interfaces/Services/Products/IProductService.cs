using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.Application.Interfaces.Services.Products
{
    /// <summary>
    /// Interface for product service operations.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Retrieves a paginated list of products with filtering and sorting.
        /// </summary>
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductParams productParams);

        /// <summary>
        /// Retrieves full product details including variants, images, and reviews.
        /// </summary>
        Task<ProductDetailsDto?> GetProductByIdAsync(int id);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        Task<bool> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a product by ID.
        /// </summary>
        Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken);
    }
}
