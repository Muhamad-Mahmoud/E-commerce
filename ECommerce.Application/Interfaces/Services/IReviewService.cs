using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Application.DTO.Reviews;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> AddReviewAsync(string userId, CreateReviewDto createReviewDto);
        Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId);
        Task<double> GetProductRatingAsync(int productId);
        Task<bool> DeleteReviewAsync(int reviewId, string userId);
    }
}
