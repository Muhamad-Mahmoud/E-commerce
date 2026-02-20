using AutoMapper;
using ECommerce.Application.DTO.Cart.Requests;
using ECommerce.Application.DTO.Cart.Responses;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    /// <summary>
    /// Service for managing shopping carts.
    /// </summary>
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ShoppingCartService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartService"/> class.
        /// </summary>
        public ShoppingCartService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ShoppingCartService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current user's shopping cart.
        /// </summary>
        public async Task<Result<ShoppingCartResponse>> GetCartAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return Result.Success(_mapper.Map<ShoppingCartResponse>(cart)!);
        }

        /// <summary>
        /// Adds an item to the user's shopping cart.
        /// </summary>
        public async Task<Result<ShoppingCartResponse>> AddItemAsync(string userId, AddToCartRequest dto)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var product = await _unitOfWork.Products.GetFirstAsync(
                p => p.Variants.Any(v => v.Id == dto.ProductVariantId),
                p => p.Variants);

            if (product == null)
            {
                _logger.LogWarning("Product variant {ProductVariantId} not found", dto.ProductVariantId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.VariantNotFound);
            }

            var variant = product.Variants.First(v => v.Id == dto.ProductVariantId);
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == dto.ProductVariantId);
            var newTotalQuantity = (existingItem?.Quantity ?? 0) + dto.Quantity;

            if (newTotalQuantity > variant.StockQuantity)
            {
                _logger.LogWarning("Insufficient stock for variant {ProductVariantId}", dto.ProductVariantId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.InsufficientStock);
            }

            cart.AddOrUpdateItem(dto.ProductVariantId, dto.Quantity);
            
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<ShoppingCartResponse>(cart)!);
        }

        /// <summary>
        /// Updates the quantity of an item in the user's shopping cart.
        /// </summary>
        public async Task<Result<ShoppingCartResponse>> UpdateItemAsync(string userId, UpdateCartItemRequest dto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.Id == dto.CartItemId);

            if (item == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found for user {UserId}", dto.CartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.ItemNotFound);
            }

            try 
            {
                item.UpdateQuantity(dto.Quantity);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success(_mapper.Map<ShoppingCartResponse>(cart)!);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<ShoppingCartResponse>(new Error("Cart.InvalidQuantity", ex.Message, 400));
            }
        }

        /// <summary>
        /// Removes an item from the user's shopping cart.
        /// </summary>
        public async Task<Result<ShoppingCartResponse>> RemoveItemAsync(string userId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found for user {UserId}", cartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.ItemNotFound);
            }

            cart.RemoveItem(item.ProductVariantId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<ShoppingCartResponse>(cart)!);
        }

        /// <summary>
        /// Clears all items from the user's shopping cart.
        /// </summary>
        public async Task<Result> ClearCartAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart.Clear();
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        private async Task<ShoppingCart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new ShoppingCart(userId);
                await _unitOfWork.ShoppingCarts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("New cart created for user {UserId}", userId);
            }
            return cart;
        }
    }
}
