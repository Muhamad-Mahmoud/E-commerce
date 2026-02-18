using ECommerce.Application.DTO.Wishlist;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<WishlistDto> GetWishlistAsync(string userId);
        Task<WishlistDto> AddToWishlistAsync(string userId, int productId);
        Task<WishlistDto> RemoveFromWishlistAsync(string userId, int productId);
        Task<bool> ClearWishlistAsync(string userId);
    }
}
