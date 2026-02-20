using ECommerce.Application.DTO.Reviews;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IReviewService
    {
        Task<Result<ReviewDto>> AddReviewAsync(string userId, CreateReviewDto createReviewDto);
        Task<Result<IEnumerable<ReviewDto>>> GetProductReviewsAsync(int productId);
        Task<Result<double>> GetProductRatingAsync(int productId);
        Task<Result> DeleteReviewAsync(int reviewId, string userId);
    }
}
