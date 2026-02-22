using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository

    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetProductReviewsAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedResult<Review>> GetProductReviewsPagedAsync(int productId, int pageNumber, int pageSize)
        {
            var query = _context.Reviews
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Review>(pageNumber, pageSize, totalCount, items);
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.ProductId == productId && r.IsApproved)
                .Select(r => r.Rating)
                .ToListAsync();

            if (ratings.Count == 0) return 0;

            return ratings.Average();
        }
    }
}
