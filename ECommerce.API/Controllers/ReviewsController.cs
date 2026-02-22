using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.DTO.Reviews;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Product review management.
    /// </summary>
    public class ReviewsController : BaseApiController
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Add a review for a product.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(CreateReviewDto createReviewDto)
        {
            return HandleResult(await _reviewService.AddReviewAsync(UserId, createReviewDto));
        }

        /// <summary>
        /// Get reviews for a specific product.
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            return HandleResult(await _reviewService.GetProductReviewsAsync(productId));
        }

        /// <summary>
        /// Get paginated reviews for a specific product.
        /// </summary>
        [HttpGet("product/{productId}/paged")]
        public async Task<IActionResult> GetProductReviewsPaged(int productId, [FromQuery] PaginationParams paginationParams)
        {
            return HandleResult(await _reviewService.GetProductReviewsPagedAsync(productId, paginationParams));
        }

        /// <summary>
        /// Get average rating for a product.
        /// </summary>
        [HttpGet("product/{productId}/rating")]
        public async Task<IActionResult> GetProductRating(int productId)
        {
            var result = await _reviewService.GetProductRatingAsync(productId);
            return result.IsSuccess
                ? Ok(new { ProductId = productId, AverageRating = result.Value })
                : HandleResult(result);
        }

        /// <summary>
        /// Delete a review.
        /// </summary>
        [HttpDelete("{reviewId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            return HandleResult(await _reviewService.DeleteReviewAsync(reviewId, UserId));
        }
    }
}
