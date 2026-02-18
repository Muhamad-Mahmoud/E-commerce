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
            return Ok(await _wishlistService.GetWishlistAsync(UserId));
        }

        /// <summary>
        /// Add product to wishlist.
        /// </summary>
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            return Ok(await _wishlistService.AddToWishlistAsync(UserId, productId));
        }

        /// <summary>
        /// Remove product from wishlist.
        /// </summary>
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            return Ok(await _wishlistService.RemoveFromWishlistAsync(UserId, productId));
        }

        /// <summary>
        /// Clear user wishlist.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearWishlist()
        {
            var result = await _wishlistService.ClearWishlistAsync(UserId);
            return result ? NoContent() : BadRequest("Could not clear wishlist");
        }
    }
}
