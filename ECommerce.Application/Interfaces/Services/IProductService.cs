using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Products.Requests;
using ECommerce.Application.DTO.Products.Responses;
using ECommerce.Domain.Shared;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Result<PagedResult<ProductResponse>>> GetProductsAsync(ProductParams productParams);

        Task<Result<ProductDetailsResponse>> GetProductByIdAsync(int id);

        Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);

        Task<Result<bool>> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken);

        Task<Result<bool>> DeleteProductAsync(int id, CancellationToken cancellationToken);
    }
}
