using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IProductVariantRepository : IRepository<ProductVariant>
    {
        // Add specific methods here if needed, like GetBySKUAsync
        Task<ProductVariant?> GetBySKUAsync(string sku);
    }
}
