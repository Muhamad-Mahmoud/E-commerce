using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// User wishlist management.
    /// </summary>
    [Authorize]
    public class WishlistController : BaseApiController
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        /// <summary>
        /// Get current user's wishlist.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            return HandleResult(await _wishlistService.GetWishlistAsync(UserId));
        }

        /// <summary>
        /// Add product to wishlist.
        /// </summary>
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            return HandleResult(await _wishlistService.AddToWishlistAsync(UserId, productId));
        }

        /// <summary>
        /// Remove product from wishlist.
        /// </summary>
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            return HandleResult(await _wishlistService.RemoveFromWishlistAsync(UserId, productId));
        }

        /// <summary>
        /// Clear user wishlist.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearWishlist()
        {
            return HandleResult(await _wishlistService.ClearWishlistAsync(UserId));
        }
    }
}
