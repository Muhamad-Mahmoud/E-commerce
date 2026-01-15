using System.Security.Claims;
using ECommerce.Application.DTO.Cart.Requests;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Manages shopping cart operations for authenticated users.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// </summary>
        /// <param name="shoppingCartService">The shopping cart service.</param>
        public CartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        /// <summary>
        /// Retrieves the current user's shopping cart.
        /// </summary>
        /// <returns>The shopping cart for the authenticated user.</returns>
        /// <response code="200">Returns the shopping cart.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _shoppingCartService.GetCartAsync(userId);
            return Ok(cart);
        }

        /// <summary>
        /// Adds an item to the user's shopping cart.
        /// If the item exists, the quantity is increased.
        /// </summary>
        /// <param name="dto">The item to add containing product variant ID and quantity.</param>
        /// <returns>The updated shopping cart.</returns>
        /// <response code="200">Item added successfully, returns updated cart.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="404">Product variant not found.</response>
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest dto)
        {
            var userId = GetUserId();
            var cart = await _shoppingCartService.AddItemAsync(userId, dto);
            return Ok(cart);
        }

        /// <summary>
        /// Updates the quantity of an item in the user's shopping cart.
        /// </summary>
        /// <param name="dto">The item update data containing cart item ID and new quantity.</param>
        /// <returns>The updated shopping cart.</returns>
        /// <response code="200">Item updated successfully, returns updated cart.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="404">Cart item not found.</response>
        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemRequest dto)
        {
            var userId = GetUserId();
            var cart = await _shoppingCartService.UpdateItemAsync(userId, dto);
            return Ok(cart);
        }

        /// <summary>
        /// Removes an item from the user's shopping cart.
        /// </summary>
        /// <param name="itemId">The ID of the cart item to remove.</param>
        /// <returns>The updated shopping cart.</returns>
        /// <response code="200">Item removed successfully, returns updated cart.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        /// <response code="404">Cart item not found.</response>
        [HttpDelete("{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var userId = GetUserId();
            var cart = await _shoppingCartService.RemoveItemAsync(userId, itemId);
            return Ok(cart);
        }

        /// <summary>
        /// Clears all items from the user's shopping cart.
        /// </summary>
        /// <returns>No content response.</returns>
        /// <response code="204">Cart cleared successfully.</response>
        /// <response code="401">Unauthorized - User not authenticated.</response>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _shoppingCartService.ClearCartAsync(userId);
            return NoContent();
        }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User ID not found in claims");
            return userId;
        }
    }
}
