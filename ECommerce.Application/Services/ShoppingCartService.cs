using AutoMapper;
using ECommerce.Application.DTO.Cart.Requests;
using ECommerce.Application.DTO.Cart.Responses;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Shared;
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

        public async Task<Result<ShoppingCartResponse>> GetCartAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return Result.Success(_mapper.Map<ShoppingCartResponse>(cart));
        }

        public async Task<Result<ShoppingCartResponse>> AddItemAsync(string userId, AddToCartRequest dto)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == dto.ProductVariantId);

            if (existingItem != null)
            {
                var existingVariant = await _unitOfWork.Products.GetFirstAsync(
                    p => p.Variants.Any(v => v.Id == dto.ProductVariantId),
                    p => p.Variants);

                if (existingVariant != null)
                {
                    var variant = existingVariant.Variants.First(v => v.Id == dto.ProductVariantId);
                    var newTotalQuantity = existingItem.Quantity + dto.Quantity;

                    if (newTotalQuantity > variant.StockQuantity)
                    {
                        _logger.LogWarning("Insufficient stock for variant {ProductVariantId}. Requested total: {Total}, Available: {Stock}",
                            dto.ProductVariantId, newTotalQuantity, variant.StockQuantity);
                        return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.InsufficientStock);
                    }
                }

                existingItem.Quantity += dto.Quantity;
                cart.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var product = await _unitOfWork.Products.GetFirstAsync(
                    p => p.Variants.Any(v => v.Id == dto.ProductVariantId),
                    p => p.Variants);

                if (product == null)
                {
                    _logger.LogWarning("Product variant {ProductVariantId} not found for user {UserId}", dto.ProductVariantId, userId);
                    return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.VariantNotFound);
                }

                var variant = product.Variants.First(v => v.Id == dto.ProductVariantId);

                if (dto.Quantity > variant.StockQuantity)
                {
                    _logger.LogWarning("Insufficient stock for variant {ProductVariantId}. Requested: {Qty}, Available: {Stock}",
                        dto.ProductVariantId, dto.Quantity, variant.StockQuantity);
                    return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.InsufficientStock);
                }

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

            return Result.Success(_mapper.Map<ShoppingCartResponse>(cart));
        }

        public async Task<Result<ShoppingCartResponse>> UpdateItemAsync(string userId, UpdateCartItemRequest dto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.Id == dto.CartItemId);

            if (item == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found for user {UserId}", dto.CartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.ItemNotFound);
            }

            item.Quantity = dto.Quantity;
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Cart item {CartItemId} updated for user {UserId}", dto.CartItemId, userId);

            return Result.Success(_mapper.Map<ShoppingCartResponse>(cart));
        }

        public async Task<Result<ShoppingCartResponse>> RemoveItemAsync(string userId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found for user {UserId}", cartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.ItemNotFound);
            }

            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Cart item {CartItemId} removed for user {UserId}", cartItemId, userId);

            return Result.Success(_mapper.Map<ShoppingCartResponse>(cart));
        }

        public async Task<Result> ClearCartAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart.Items.Clear();
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Cart cleared for user {UserId}", userId);
            return Result.Success();
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
