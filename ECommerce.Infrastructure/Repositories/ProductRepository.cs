using ECommerce.Application.DTO;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
// using ECommerce.Domain.Interfaces.Repositories; // REMOVED (moved to Application)
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    /// <summary>
    /// Product repository implementation.
    /// </summary>
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Product?> GetWithVariantsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Product?> GetWithFullDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetPublishedProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.Status == ProductStatus.Published)
                .Include(p => p.Variants)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Variants)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Product>> SearchProductsAsync(ProductParams p, CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .Include(x => x.Variants)
                .AsQueryable();

            // 1. Filtering
            if (!string.IsNullOrEmpty(p.Search))
            {
                var lowerTerm = p.Search.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(lowerTerm) || 
                                       (x.Description != null && x.Description.ToLower().Contains(lowerTerm)));
            }

            if (p.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == p.CategoryId);
            }

            if (p.MinPrice.HasValue)
            {
                query = query.Where(x => x.Variants.Any(v => v.Price >= p.MinPrice));
            }

            if (p.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Variants.Any(v => v.Price <= p.MaxPrice));
            }

            // 2. Sorting
            query = p.Sort switch
            {
                "priceAsc" => query.OrderBy(x => x.Variants.Min(v => v.Price)),
                "priceDesc" => query.OrderByDescending(x => x.Variants.Min(v => v.Price)),
                "newest" => query.OrderByDescending(x => x.Id), // Or CreatedAt if available
                _ => query.OrderBy(x => x.Name)
            };

            // 3. Pagination
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((p.PageNumber - 1) * p.PageSize)
                .Take(p.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = p.PageNumber,
                PageSize = p.PageSize
            };
        }
    }
}

