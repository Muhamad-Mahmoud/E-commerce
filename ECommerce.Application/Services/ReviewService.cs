using AutoMapper;
using ECommerce.Application.DTO.Reviews;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Shared;

namespace ECommerce.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ReviewDto>> AddReviewAsync(string userId, CreateReviewDto createReviewDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(createReviewDto.ProductId);
            if (product == null)
                return Result.Failure<ReviewDto>(DomainErrors.Review.ProductNotFound);

            var existingReview = await _unitOfWork.Reviews.GetFirstAsync(
                r => r.ProductId == createReviewDto.ProductId && r.UserId == userId);

            if (existingReview != null)
                return Result.Failure<ReviewDto>(DomainErrors.Review.DuplicateReview);

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

            return Result.Success(_mapper.Map<ReviewDto>(review));
        }

        public async Task<Result<IEnumerable<ReviewDto>>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetProductReviewsAsync(productId);
            return Result.Success(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
        }

        public async Task<Result<double>> GetProductRatingAsync(int productId)
        {
            var rating = await _unitOfWork.Reviews.GetAverageRatingAsync(productId);
            return Result.Success(rating);
        }

        public async Task<Result> DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
                return Result.Failure(DomainErrors.Review.NotFound);

            if (review.UserId != userId)
                return Result.Failure(DomainErrors.Review.Unauthorized);

            _unitOfWork.Reviews.Delete(review);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
