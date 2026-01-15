using AutoMapper;
using ECommerce.Application.DTO.Cart.Requests;
using ECommerce.Application.DTO.Cart.Responses;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ShoppingCartService> _logger;

        public ShoppingCartService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ShoppingCartService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ShoppingCartResponse> GetCartAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required", nameof(userId));

            var cart = await GetOrCreateCartAsync(userId);
            return _mapper.Map<ShoppingCartResponse>(cart);
        }

        public async Task<ShoppingCartResponse> AddItemAsync(string userId, AddToCartRequest dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required", nameof(userId));

            var cart = await GetOrCreateCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == dto.ProductVariantId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                cart.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var product = await _unitOfWork.Products.GetFirstAsync(p => p.Variants.Any(v => v.Id == dto.ProductVariantId), p => p.Variants);

                if (product == null)
                {
                    _logger.LogWarning("Product variant {ProductVariantId} not found for user {UserId}", dto.ProductVariantId, userId);
                    throw new InvalidOperationException("Product variant not found");
                }

                var variant = product.Variants.First(v => v.Id == dto.ProductVariantId);

                cart.Items.Add(new ShoppingCartItem
                {
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity,
                    UnitPrice = variant.Price
                });
                cart.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation("Item {ProductVariantId} added to cart for user {UserId}", dto.ProductVariantId, userId);
            }

            await _unitOfWork.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task<ShoppingCartResponse> UpdateItemAsync(string userId, UpdateCartItemRequest dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required", nameof(userId));

            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.Id == dto.CartItemId);

            if (item == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found for user {UserId}", dto.CartItemId, userId);
                throw new InvalidOperationException("Cart item not found");
            }

            item.Quantity = dto.Quantity;
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Cart item {CartItemId} updated for user {UserId}", dto.CartItemId, userId);

            return await GetCartAsync(userId);
        }

        public async Task<ShoppingCartResponse> RemoveItemAsync(string userId, int cartItemId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required", nameof(userId));

            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found for user {UserId}", cartItemId, userId);
                throw new InvalidOperationException("Cart item not found");
            }

            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Cart item {CartItemId} removed for user {UserId}", cartItemId, userId);

            return await GetCartAsync(userId);
        }

        public async Task ClearCartAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required", nameof(userId));

            var cart = await GetOrCreateCartAsync(userId);
            cart.Items.Clear();
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Cart cleared for user {UserId}", userId);
        }

        private async Task<ShoppingCart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Items = new List<ShoppingCartItem>()
                };
                await _unitOfWork.ShoppingCarts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("New cart created for user {UserId}", userId);
            }
            return cart;
        }
    }
}
