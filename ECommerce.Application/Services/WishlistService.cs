using AutoMapper;
using ECommerce.Application.DTO.Wishlist;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    /// <summary>
    /// Service for managing user wishlists.
    /// </summary>
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WishlistService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WishlistService"/> class.
        /// </summary>
        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WishlistService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current user's wishlist.
        /// </summary>
        public async Task<Result<WishlistDto>> GetWishlistAsync(string userId)
        {
            var result = await GetOrCreateWishlistAsync(userId);
            if (result.IsFailure) return Result.Failure<WishlistDto>(result.Error);

            return Result.Success(_mapper.Map<WishlistDto>(result.Value)!);
        }

        /// <summary>
        /// Adds a product to the user's wishlist.
        /// </summary>
        public async Task<Result<WishlistDto>> AddToWishlistAsync(string userId, int productId)
        {
            var result = await GetOrCreateWishlistAsync(userId);
            if (result.IsFailure) return Result.Failure<WishlistDto>(result.Error);

            var wishlist = result.Value;

            if (!wishlist.Items.Any(i => i.ProductId == productId))
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found to add to wishlist for user {UserId}", productId, userId);
                    return Result.Failure<WishlistDto>(DomainErrors.Product.NotFound);
                }

                try 
                {
                    wishlist.Items.Add(new WishlistItem { ProductId = productId });
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Product {ProductId} added to wishlist for user {UserId}", productId, userId);
                }
                catch (ConcurrencyConflictException ex)
                {
                    _logger.LogWarning(ex, "Concurrency conflict adding product {ProductId} to wishlist for user {UserId}", productId, userId);
                    return Result.Failure<WishlistDto>(DomainErrors.Order.ConcurrencyConflict);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding product {ProductId} to wishlist for user {UserId}", productId, userId);
                    return Result.Failure<WishlistDto>(DomainErrors.General.ServerError);
                }
            }

            return Result.Success(_mapper.Map<WishlistDto>(wishlist)!);
        }

        /// <summary>
        /// Removes a product from the user's wishlist.
        /// </summary>
        public async Task<Result<WishlistDto>> RemoveFromWishlistAsync(string userId, int productId)
        {
            var result = await GetOrCreateWishlistAsync(userId);
            if (result.IsFailure) return Result.Failure<WishlistDto>(result.Error);

            var wishlist = result.Value;
            var item = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                _logger.LogWarning("Product {ProductId} not found in wishlist for user {UserId}", productId, userId);
                return Result.Failure<WishlistDto>(DomainErrors.Wishlist.ItemNotFound);
            }

            try 
            {
                wishlist.Items.Remove(item);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Product {ProductId} removed from wishlist for user {UserId}", productId, userId);
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict removing product {ProductId} from wishlist for user {UserId}", productId, userId);
                return Result.Failure<WishlistDto>(DomainErrors.Order.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product {ProductId} from wishlist for user {UserId}", productId, userId);
                return Result.Failure<WishlistDto>(DomainErrors.General.ServerError);
            }

            return Result.Success(_mapper.Map<WishlistDto>(wishlist)!);
        }

        /// <summary>
        /// Clears all products from the user's wishlist.
        /// </summary>
        public async Task<Result> ClearWishlistAsync(string userId)
        {
            var result = await GetOrCreateWishlistAsync(userId);
            if (result.IsFailure) return result;

            var wishlist = result.Value;
            if (wishlist.Items.Any())
            {
                try 
                {
                    wishlist.Items.Clear();
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Wishlist cleared for user {UserId}", userId);
                }
                catch (ConcurrencyConflictException ex)
                {
                    _logger.LogWarning(ex, "Concurrency conflict clearing wishlist for user {UserId}", userId);
                    return Result.Failure(DomainErrors.Order.ConcurrencyConflict);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error clearing wishlist for user {UserId}", userId);
                    return Result.Failure(DomainErrors.General.ServerError);
                }
            }
            return Result.Success();
        }

        private async Task<Result<Wishlist>> GetOrCreateWishlistAsync(string userId)
        {
            var wishlist = await _unitOfWork.Wishlists.GetWishlistByUserIdAsync(userId);
            if (wishlist == null)
            {
                try 
                {
                    wishlist = new Wishlist { UserId = userId };
                    await _unitOfWork.Wishlists.AddAsync(wishlist);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("New wishlist created for user {UserId}", userId);
                }
                catch (ConcurrencyConflictException)
                {
                    _logger.LogInformation("Wishlist for user {UserId} was created by another process, reloading", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating wishlist for user {UserId}", userId);
                    return Result.Failure<Wishlist>(DomainErrors.General.ServerError);
                }

                wishlist = await _unitOfWork.Wishlists.GetWishlistByUserIdAsync(userId);
            }

            return wishlist != null 
                ? Result.Success(wishlist) 
                : Result.Failure<Wishlist>(DomainErrors.General.ServerError);
        }
    }
}
