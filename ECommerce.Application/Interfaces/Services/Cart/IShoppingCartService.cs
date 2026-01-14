using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Interfaces.Services.Cart
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto> GetCartAsync(string userId);
        Task<ShoppingCartDto> AddItemAsync(string userId, AddToCartDto dto);
        Task<ShoppingCartDto> UpdateItemAsync(string userId, UpdateCartItemDto dto);
        Task<ShoppingCartDto> RemoveItemAsync(string userId, int cartItemId);
        Task ClearCartAsync(string userId);
    }
}
