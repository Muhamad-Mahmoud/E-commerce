using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Application.DTO.Reviews;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

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

        public async Task<ReviewDto> AddReviewAsync(string userId, CreateReviewDto createReviewDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(createReviewDto.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            var review = new Review
            {
                ProductId = createReviewDto.ProductId,
                UserId = userId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment,
                Title = createReviewDto.Title,
                CreatedAt = DateTime.UtcNow,
                IsApproved = true // Automatically approve for now
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetProductReviewsAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<double> GetProductRatingAsync(int productId)
        {
            return await _unitOfWork.Reviews.GetAverageRatingAsync(productId);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null) return false;

            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only delete your own reviews");
            }

            _unitOfWork.Reviews.Delete(review);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
