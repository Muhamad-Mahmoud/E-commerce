using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Domain.Interfaces.Repositories
{
    public interface IReviewRepository : IRepository<Review>

    {
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
        Task<PagedResult<Review>> GetProductReviewsPagedAsync(int productId, int pageNumber, int pageSize);
        Task<double> GetAverageRatingAsync(int productId);
    }
}
