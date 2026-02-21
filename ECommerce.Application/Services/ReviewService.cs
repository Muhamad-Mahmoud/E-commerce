using AutoMapper;
using ECommerce.Application.DTO.Reviews;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReviewService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new review for a product.
        /// </summary>
        public async Task<Result<ReviewDto>> AddReviewAsync(string userId, CreateReviewDto createReviewDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(createReviewDto.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for review by user {UserId}", createReviewDto.ProductId, userId);
                return Result.Failure<ReviewDto>(DomainErrors.Review.ProductNotFound);
            }

            var existingReview = await _unitOfWork.Reviews.GetFirstAsync(
                r => r.ProductId == createReviewDto.ProductId && r.UserId == userId);

            if (existingReview != null)
            {
                _logger.LogWarning("User {UserId} attempted to add duplicate review for product {ProductId}", userId, createReviewDto.ProductId);
                return Result.Failure<ReviewDto>(DomainErrors.Review.DuplicateReview);
            }

            try 
            {
                var review = new Review
                {
                    ProductId = createReviewDto.ProductId,
                    UserId = userId,
                    Rating = createReviewDto.Rating,
                    Comment = createReviewDto.Comment,
                    Title = createReviewDto.Title,
                    CreatedAt = DateTime.UtcNow,
                    IsApproved = false
                };

                await _unitOfWork.Reviews.AddAsync(review);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("New review {ReviewId} added for product {ProductId} by user {UserId}", review.Id, createReviewDto.ProductId, userId);
                return Result.Success(_mapper.Map<ReviewDto>(review)!);
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict adding review for product {ProductId} by user {UserId}", createReviewDto.ProductId, userId);
                return Result.Failure<ReviewDto>(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review for product {ProductId} by user {UserId}", createReviewDto.ProductId, userId);
                return Result.Failure<ReviewDto>(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Gets all reviews for a specific product.
        /// </summary>
        public async Task<Result<IEnumerable<ReviewDto>>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetProductReviewsAsync(productId);
            return Result.Success(_mapper.Map<IEnumerable<ReviewDto>>(reviews) ?? Enumerable.Empty<ReviewDto>());
        }

        /// <summary>
        /// Gets the average rating for a specific product.
        /// </summary>
        public async Task<Result<double>> GetProductRatingAsync(int productId)
        {
            var rating = await _unitOfWork.Reviews.GetAverageRatingAsync(productId);
            return Result.Success(rating);
        }

        /// <summary>
        /// Deletes a user's review.
        /// </summary>
        public async Task<Result> DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
            {
                _logger.LogWarning("Review {ReviewId} not found for deletion by user {UserId}", reviewId, userId);
                return Result.Failure(DomainErrors.Review.NotFound);
            }

            if (review.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted unauthorized deletion of review {ReviewId}", userId, reviewId);
                return Result.Failure(DomainErrors.Review.Unauthorized);
            }

            try 
            {
                _unitOfWork.Reviews.Delete(review);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Review {ReviewId} deleted by user {UserId}", reviewId, userId);
                return Result.Success();
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict deleting review {ReviewId} by user {UserId}", reviewId, userId);
                return Result.Failure(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId} by user {UserId}", reviewId, userId);
                return Result.Failure(DomainErrors.General.ServerError);
            }
        }
    }
}
