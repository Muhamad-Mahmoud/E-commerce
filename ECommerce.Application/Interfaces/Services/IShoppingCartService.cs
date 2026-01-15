using ECommerce.Application.DTO.Cart.Requests;
using ECommerce.Application.DTO.Cart.Responses;

namespace ECommerce.Application.Interfaces.Services
{
    /// <summary>
    /// Interface for shopping cart service operations.
    /// </summary>
    public interface IShoppingCartService
    {
        /// <summary>
        /// Retrieves the shopping cart for a specific user.
        /// </summary>
        Task<ShoppingCartResponse> GetCartAsync(string userId);

        /// <summary>
        /// Adds an item to the user's shopping cart. If item exists, quantity is incremented.
        /// </summary>
        Task<ShoppingCartResponse> AddItemAsync(string userId, AddToCartRequest dto);

        /// <summary>
        /// Updates the quantity of an existing cart item.
        /// </summary>
        Task<ShoppingCartResponse> UpdateItemAsync(string userId, UpdateCartItemRequest dto);

        /// <summary>
        /// Removes an item from the user's shopping cart.
        /// </summary>
        Task<ShoppingCartResponse> RemoveItemAsync(string userId, int cartItemId);

        /// <summary>
        /// Clears all items from the user's shopping cart.
        /// </summary>
        Task ClearCartAsync(string userId);
    }
}
