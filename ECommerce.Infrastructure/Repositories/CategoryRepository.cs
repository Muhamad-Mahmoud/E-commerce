using ECommerce.Domain.Entities;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    /// <summary>
    /// Category repository implementation.
    /// </summary>
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
        }

        public async Task<PagedResult<CategoryDto>> SearchCategoriesAsync(CategoryParams p, CancellationToken cancellationToken = default)
        {
            // Start Query
            var query = _context.Categories
                .AsNoTracking()
                .AsQueryable();

            //  Filtering
            if (!string.IsNullOrEmpty(p.Search))
            {
                var lowerTerm = p.Search.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(lowerTerm));
            }

            if (p.ParentCategoryId.HasValue)
            {
                query = query.Where(c => c.ParentCategoryId == p.ParentCategoryId.Value);
            }
            else if (p.IncludeSubCategories == false)
            {
                // Only root categories
                query = query.Where(c => c.ParentCategoryId == null);
            }

            // Sorting
            query = query.OrderBy(c => c.ParentCategoryId)
                        .ThenBy(c => c.Name);

            //  Pagination & Count
            var totalCount = await query.CountAsync(cancellationToken);

            // Projection (Select DTO directly)
            var items = await query
                .Skip((p.PageNumber - 1) * p.PageSize)
                .Take(p.PageSize)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<CategoryDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = p.PageNumber,
                PageSize = p.PageSize
            };
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await FindAsync(c => c.ParentCategoryId == null, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(c => c.ParentCategoryId == parentId, cancellationToken);
        }

        public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            var category = await FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
            return category != null;
        }
    }
}
