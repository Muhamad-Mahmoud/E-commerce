using ECommerce.Application.DTO.Cart.Requests;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Shopping cart management.
    /// </summary>
    [Authorize]
    public class CartController : BaseApiController
    {
        private readonly IShoppingCartService _shoppingCartService;

        public CartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        /// <summary>
        /// Get current user's cart.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            return HandleResult(await _shoppingCartService.GetCartAsync(UserId));
        }

        /// <summary>
        /// Add item to cart.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest dto)
        {
            return HandleResult(await _shoppingCartService.AddItemAsync(UserId, dto));
        }

        /// <summary>
        /// Update cart item quantity.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemRequest dto)
        {
            return HandleResult(await _shoppingCartService.UpdateItemAsync(UserId, dto));
        }

        /// <summary>
        /// Remove item from cart.
        /// </summary>
        [HttpDelete("{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            return HandleResult(await _shoppingCartService.RemoveItemAsync(UserId, itemId));
        }

        /// <summary>
        /// Clear user cart.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            return HandleResult(await _shoppingCartService.ClearCartAsync(UserId));
        }
    }
}
