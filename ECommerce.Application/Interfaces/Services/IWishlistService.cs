using ECommerce.Application.DTO.Wishlist;
using ECommerce.Domain.Shared;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<Result<WishlistDto>> GetWishlistAsync(string userId);
        Task<Result<WishlistDto>> AddToWishlistAsync(string userId, int productId);
        Task<Result<WishlistDto>> RemoveFromWishlistAsync(string userId, int productId);
        Task<Result> ClearWishlistAsync(string userId);
    }
}
