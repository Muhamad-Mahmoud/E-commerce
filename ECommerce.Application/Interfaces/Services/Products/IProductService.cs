using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;

namespace ECommerce.Application.Interfaces.Services.Products
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductParams productParams);
        Task<ProductDetailsDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductRequest request);
        Task<bool> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(int id);
    }
}
