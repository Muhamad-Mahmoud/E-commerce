using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Products.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetWithVariantsAsync(int id);
        Task<Product?> GetWithFullDetailsAsync(int id);
        Task<PagedResult<ProductResponse>> SearchProductsAsync(ProductParams productParams);
        Task<IEnumerable<Product>> GetPublishedProductsAsync();
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    }
}
