using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Products.Requests;
using ECommerce.Application.DTO.Products.Responses;

namespace ECommerce.Application.Interfaces.Services
{
    /// <summary>
    /// Interface for product service operations.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Retrieves a paginated list of products with filtering and sorting.
        /// </summary>
        Task<PagedResult<ProductResponse>> GetProductsAsync(ProductParams productParams);

        /// <summary>
        /// Retrieves full product details including variants, images, and reviews.
        /// </summary>
        Task<ProductDetailsResponse?> GetProductByIdAsync(int id);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        Task<ProductResponse> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);

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
