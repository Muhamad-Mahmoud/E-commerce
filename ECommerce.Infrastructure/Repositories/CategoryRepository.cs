using ECommerce.Domain.Entities;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await GetFirstAsync(c => c.Name == name);
        }

        public async Task<PagedResult<CategoryDto>> SearchCategoriesAsync(CategoryParams p)
        {
            var query = _context.Categories
                .AsNoTracking();

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
                query = query.Where(c => c.ParentCategoryId == null);
            }

            // Sorting
            query = query.OrderBy(c => c.ParentCategoryId)
                        .ThenBy(c => c.Name);

            var totalCount = await query.CountAsync();

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
                .ToListAsync();

            return new PagedResult<CategoryDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = p.PageNumber,
                PageSize = p.PageSize
            };
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            return await FindAsync(c => c.ParentCategoryId == null);
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId)
        {
            return await FindAsync(c => c.ParentCategoryId == parentId);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            var category = await GetFirstAsync(c => c.Name == name);
            return category != null;
        }
    }
}
