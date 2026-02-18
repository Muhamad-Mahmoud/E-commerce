using AutoMapper;
using ECommerce.Application.DTO.Wishlist;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

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

        public async Task<WishlistDto> GetWishlistAsync(string userId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            return _mapper.Map<WishlistDto>(wishlist);
        }

        public async Task<WishlistDto> AddToWishlistAsync(string userId, int productId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            
            if (!wishlist.Items.Any(i => i.ProductId == productId))
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null) throw new KeyNotFoundException("Product not found");

                wishlist.Items.Add(new WishlistItem
                {
                    ProductId = productId
                });

                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<WishlistDto>(wishlist);
        }

        public async Task<WishlistDto> RemoveFromWishlistAsync(string userId, int productId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            var item = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                wishlist.Items.Remove(item);
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.Map<WishlistDto>(wishlist);
        }

        public async Task<bool> ClearWishlistAsync(string userId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            if (wishlist.Items.Any())
            {
                wishlist.Items.Clear();
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            return true;
        }

        private async Task<Wishlist> GetOrCreateWishlistAsync(string userId)
        {
            var wishlist = await _unitOfWork.Wishlists.GetWishlistByUserIdAsync(userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId };
                await _unitOfWork.Wishlists.AddAsync(wishlist);
                await _unitOfWork.SaveChangesAsync();
                
                // Reload to get includes if any (though new wishlist is empty)
                wishlist = await _unitOfWork.Wishlists.GetWishlistByUserIdAsync(userId);
            }
            return wishlist!;
        }
    }
}
