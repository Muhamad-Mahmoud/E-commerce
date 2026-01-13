using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.Application.Interfaces.Services.Products
{
    public interface IProductService
    {
        /// <summary>
        /// Retrieves paginated list of products with filtering options.
        /// </summary>
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductParams productParams);
        /// <summary>
        /// Retrieves full product details including variants, images, and reviews.
        /// </summary>
        Task<ProductDetailsDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);
        Task<bool> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken);
    }
}
