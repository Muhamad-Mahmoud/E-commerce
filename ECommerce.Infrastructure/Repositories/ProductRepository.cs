using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Products.Responses;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetWithVariantsAsync(int id)
        {
            return await GetByIdAsync(id, p => p.Variants);
        }

        public async Task<Product?> GetWithFullDetailsAsync(int id)
        {
            return await GetByIdAsync(id,
                p => p.Category,
                p => p.Variants,
                p => p.Images,
                p => p.Reviews);
        }

        public async Task<IEnumerable<Product>> GetPublishedProductsAsync()
        {
            return await FindAsync(p => p.Status == ProductStatus.Published, p => p.Variants);
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await FindAsync(p => p.CategoryId == categoryId, p => p.Variants);
        }

        public async Task<PagedResult<ProductResponse>> SearchProductsAsync(ProductParams p)
        {
            var query = _context.Products
                .AsNoTracking()
                .AsQueryable();

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
                query = query.Where(x => x.Variants.Min(v => v.Price) >= p.MinPrice);
            }

            if (p.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Variants.Min(v => v.Price) <= p.MaxPrice);
            }

            query = p.Sort switch
            {
                ProductParams.SortPriceAsc => query.OrderBy(x => x.Variants.Min(v => v.Price)),
                ProductParams.SortPriceDesc => query.OrderByDescending(x => x.Variants.Min(v => v.Price)),
                ProductParams.SortNewest => query.OrderByDescending(x => x.Id),
                _ => query.OrderBy(x => x.Name)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((p.PageNumber - 1) * p.PageSize)
                .Take(p.PageSize)
                .Select(pr => new ProductResponse
                {
                    Id = pr.Id,
                    Name = pr.Name,
                    Description = pr.Description,
                    CategoryName = pr.Category != null ? pr.Category.Name : null,
                    Price = pr.Variants.Any() ? pr.Variants.Min(v => v.Price) : 0,
                    MainImageUrl = pr.Images.Any(i => i.IsPrimary)
                        ? pr.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl
                        : (pr.Images.Any() ? pr.Images.FirstOrDefault()!.ImageUrl : null)
                })
                .ToListAsync();

            return new PagedResult<ProductResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = p.PageNumber,
                PageSize = p.PageSize
            };
        }
    }
}
