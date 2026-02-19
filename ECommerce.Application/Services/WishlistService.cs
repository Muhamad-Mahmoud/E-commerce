using AutoMapper;
using ECommerce.Application.DTO.Wishlist;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Shared;

namespace ECommerce.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<WishlistDto>> GetWishlistAsync(string userId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            return Result.Success(_mapper.Map<WishlistDto>(wishlist));
        }

        public async Task<Result<WishlistDto>> AddToWishlistAsync(string userId, int productId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);

            if (!wishlist.Items.Any(i => i.ProductId == productId))
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                    return Result.Failure<WishlistDto>(DomainErrors.Product.NotFound);

                wishlist.Items.Add(new WishlistItem { ProductId = productId });
                await _unitOfWork.SaveChangesAsync();
            }

            return Result.Success(_mapper.Map<WishlistDto>(wishlist));
        }

        public async Task<Result<WishlistDto>> RemoveFromWishlistAsync(string userId, int productId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            var item = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                return Result.Failure<WishlistDto>(DomainErrors.Wishlist.ItemNotFound);

            wishlist.Items.Remove(item);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(_mapper.Map<WishlistDto>(wishlist));
        }

        public async Task<Result> ClearWishlistAsync(string userId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            if (wishlist.Items.Any())
            {
                wishlist.Items.Clear();
                await _unitOfWork.SaveChangesAsync();
            }
            return Result.Success();
        }

        private async Task<Wishlist> GetOrCreateWishlistAsync(string userId)
        {
            var wishlist = await _unitOfWork.Wishlists.GetWishlistByUserIdAsync(userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId };
                await _unitOfWork.Wishlists.AddAsync(wishlist);
                await _unitOfWork.SaveChangesAsync();

                // Reload to get any includes on the fresh entity
                wishlist = await _unitOfWork.Wishlists.GetWishlistByUserIdAsync(userId);
            }
            return wishlist!;
        }
    }
}
