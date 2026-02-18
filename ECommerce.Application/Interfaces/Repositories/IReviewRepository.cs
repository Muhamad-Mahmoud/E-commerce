using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces.Repositories
{
    public interface IReviewRepository : IRepository<Review>

    {
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
        Task<double> GetAverageRatingAsync(int productId);
    }
}
