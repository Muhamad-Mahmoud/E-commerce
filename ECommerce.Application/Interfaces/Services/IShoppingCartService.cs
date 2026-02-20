using ECommerce.Application.DTO.Cart.Requests;
using ECommerce.Application.DTO.Cart.Responses;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IShoppingCartService
    {
        Task<Result<ShoppingCartResponse>> GetCartAsync(string userId);
        Task<Result<ShoppingCartResponse>> AddItemAsync(string userId, AddToCartRequest dto);
        Task<Result<ShoppingCartResponse>> UpdateItemAsync(string userId, UpdateCartItemRequest dto);
        Task<Result<ShoppingCartResponse>> RemoveItemAsync(string userId, int cartItemId);
        Task<Result> ClearCartAsync(string userId);
    }
}
