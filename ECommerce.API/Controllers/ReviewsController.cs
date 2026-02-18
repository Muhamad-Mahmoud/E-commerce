using ECommerce.Application.DTO.Reviews;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    public class ReviewsController : BaseApiController
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(CreateReviewDto createReviewDto)
        {
            var review = await _reviewService.AddReviewAsync(UserId, createReviewDto);
            return Ok(review);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _reviewService.GetProductReviewsAsync(productId);
            return Ok(reviews);
        }

        [HttpGet("product/{productId}/rating")]
        public async Task<IActionResult> GetProductRating(int productId)
        {
            var rating = await _reviewService.GetProductRatingAsync(productId);
            return Ok(new { ProductId = productId, AverageRating = rating });
        }

        [HttpDelete("{reviewId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var result = await _reviewService.DeleteReviewAsync(reviewId, UserId);
            return result ? NoContent() : NotFound();
        }
    }
}
