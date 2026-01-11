using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.Repositories;
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

        public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Product>();

            var lowerSearchTerm = searchTerm.ToLower();

            return await _context.Products
                .Where(p => p.Name.ToLower().Contains(lowerSearchTerm) ||
                           (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm)))
                .Include(p => p.Variants)
                .ToListAsync(cancellationToken);
        }
    }
}

