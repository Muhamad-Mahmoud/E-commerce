using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ProductVariant?> GetBySKUAsync(string sku)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.SKU == sku);
        }
    }
}
