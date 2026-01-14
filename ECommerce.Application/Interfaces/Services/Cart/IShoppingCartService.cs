using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Interfaces.Services.Cart
{
    /// <summary>
    /// Interface for shopping cart service operations.
    /// </summary>
    public interface IShoppingCartService
    {
        /// <summary>
        /// Retrieves the shopping cart for a specific user.
        /// </summary>
        Task<ShoppingCartDto> GetCartAsync(string userId);

        /// <summary>
        /// Adds an item to the user's shopping cart. If item exists, quantity is incremented.
        /// </summary>
        Task<ShoppingCartDto> AddItemAsync(string userId, AddToCartDto dto);

        /// <summary>
        /// Updates the quantity of an existing cart item.
        /// </summary>
        Task<ShoppingCartDto> UpdateItemAsync(string userId, UpdateCartItemDto dto);

        /// <summary>
        /// Removes an item from the user's shopping cart.
        /// </summary>
        Task<ShoppingCartDto> RemoveItemAsync(string userId, int cartItemId);

        /// <summary>
        /// Clears all items from the user's shopping cart.
        /// </summary>
        Task ClearCartAsync(string userId);
    }
}
