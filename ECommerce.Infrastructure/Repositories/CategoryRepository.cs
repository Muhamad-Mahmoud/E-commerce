using ECommerce.Domain.Entities;
using ECommerce.Application.Interfaces.Repositories;
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

        public override async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.ParentCategoryId)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name == name, cancellationToken);
        }
    }
}
