using ECommerce.Application.DTO.Products;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
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

        public async Task<Product?> GetWithVariantsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(id, cancellationToken, p => p.Variants);
        }

        public async Task<Product?> GetWithFullDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(id, cancellationToken, 
                p => p.Category,
                p => p.Variants, 
                p => p.Images, 
                p => p.Reviews);
        }

        public async Task<IEnumerable<Product>> GetPublishedProductsAsync(CancellationToken cancellationToken = default)
        {
            return await FindAsync(p => p.Status == ProductStatus.Published, cancellationToken, p => p.Variants);
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(p => p.CategoryId == categoryId, cancellationToken, p => p.Variants);
        }

        public async Task<PagedResult<ProductDto>> SearchProductsAsync(ProductParams p, CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .AsQueryable();

            // Filtering
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

            // Price filtering using Min Variant Price
            if (p.MinPrice.HasValue)
            {
                query = query.Where(x => x.Variants.Min(v => v.Price) >= p.MinPrice);
            }

            if (p.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Variants.Min(v => v.Price) <= p.MaxPrice);
            }

            //  Sorting
            query = p.Sort switch
            {
                "priceAsc" => query.OrderBy(x => x.Variants.Min(v => v.Price)),
                "priceDesc" => query.OrderByDescending(x => x.Variants.Min(v => v.Price)),
                "newest" => query.OrderByDescending(x => x.Id),
                _ => query.OrderBy(x => x.Name)
            };

            // Pagination & Count
            var totalCount = await query.CountAsync(cancellationToken);

            //  Projection to DTO
            var items = await query
                .Skip((p.PageNumber - 1) * p.PageSize)
                .Take(p.PageSize)
                .Select(pr => new ProductDto
                {
                    Id = pr.Id,
                    Name = pr.Name,
                    Description = pr.Description,
                    CategoryName = pr.Category != null ? pr.Category.Name : null,
                    // Get minimum price from variants
                    Price = pr.Variants.Any() ? pr.Variants.Min(v => v.Price) : 0,
                    // Get Primary Image, or first image, or null
                    MainImageUrl = pr.Images.Any(i => i.IsPrimary) 
                        ? pr.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl 
                        : (pr.Images.Any() ? pr.Images.FirstOrDefault()!.ImageUrl : null)
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = p.PageNumber,
                PageSize = p.PageSize
            };
        }
    }
}
