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
            var result = await GetOrCreateCartAsync(userId);
            if (result.IsFailure) return Result.Failure<ShoppingCartResponse>(result.Error);

            return Result.Success(_mapper.Map<ShoppingCartResponse>(result.Value)!);
        }

        /// <summary>
        /// Adds an item to the user's shopping cart.
        /// </summary>
        public async Task<Result<ShoppingCartResponse>> AddItemAsync(string userId, AddToCartRequest dto)
        {
            var result = await GetOrCreateCartAsync(userId);
            if (result.IsFailure) return Result.Failure<ShoppingCartResponse>(result.Error);

            var cart = result.Value;

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

            try 
            {
                cart.AddOrUpdateItem(dto.ProductVariantId, dto.Quantity);
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Item {ProductVariantId} added/updated in cart for user {UserId}", dto.ProductVariantId, userId);
                return Result.Success(_mapper.Map<ShoppingCartResponse>(cart)!);
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict adding item to cart for user {UserId}", userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item {ProductVariantId} to cart for user {UserId}", dto.ProductVariantId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Updates the quantity of an item in the user's shopping cart.
        /// </summary>
        public async Task<Result<ShoppingCartResponse>> UpdateItemAsync(string userId, UpdateCartItemRequest dto)
        {
            var result = await GetOrCreateCartAsync(userId);
            if (result.IsFailure) return Result.Failure<ShoppingCartResponse>(result.Error);

            var cart = result.Value;
            var item = cart.Items.FirstOrDefault(i => i.Id == dto.CartItemId);

            if (item == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found for user {UserId}", dto.CartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.ItemNotFound);
            }

            // Stock Validation
            if (dto.Quantity > item.ProductVariant.StockQuantity)
            {
                _logger.LogWarning("Insufficient stock for variant {ProductVariantId}. Requested: {Quantity}, Available: {StockQuantity}", 
                    item.ProductVariantId, dto.Quantity, item.ProductVariant.StockQuantity);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.InsufficientStock);
            }

            try 
            {
                item.UpdateQuantity(dto.Quantity);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Cart item {CartItemId} quantity updated to {Quantity} for user {UserId}", dto.CartItemId, dto.Quantity, userId);
                return Result.Success(_mapper.Map<ShoppingCartResponse>(cart)!);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<ShoppingCartResponse>(new Error("Cart.InvalidQuantity", ex.Message, 400));
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict updating cart item {CartItemId} for user {UserId}", dto.CartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId} for user {UserId}", dto.CartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Removes an item from the user's shopping cart.
        /// </summary>
        public async Task<Result<ShoppingCartResponse>> RemoveItemAsync(string userId, int cartItemId)
        {
            var result = await GetOrCreateCartAsync(userId);
            if (result.IsFailure) return Result.Failure<ShoppingCartResponse>(result.Error);

            var cart = result.Value;
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found for user {UserId}", cartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.Cart.ItemNotFound);
            }

            try 
            {
                cart.RemoveItem(item.ProductVariantId);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Item {ProductVariantId} removed from cart for user {UserId}", item.ProductVariantId, userId);
                return Result.Success(_mapper.Map<ShoppingCartResponse>(cart)!);
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict removing cart item {CartItemId} for user {UserId}", cartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartItemId} for user {UserId}", cartItemId, userId);
                return Result.Failure<ShoppingCartResponse>(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Clears all items from the user's shopping cart.
        /// </summary>
        public async Task<Result> ClearCartAsync(string userId)
        {
            var result = await GetOrCreateCartAsync(userId);
            if (result.IsFailure) return result;

            var cart = result.Value;
            try 
            {
                cart.Clear();
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Cart cleared for user {UserId}", userId);
                return Result.Success();
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict clearing cart for user {UserId}", userId);
                return Result.Failure(DomainErrors.General.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                return Result.Failure(DomainErrors.General.ServerError);
            }
        }

        private async Task<Result<ShoppingCart>> GetOrCreateCartAsync(string userId)
        {
            var cart = await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
            if (cart == null)
            {
                try 
                {
                    cart = new ShoppingCart(userId);
                    await _unitOfWork.ShoppingCarts.AddAsync(cart);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("New cart created for user {UserId}", userId);
                }
                catch (ConcurrencyConflictException)
                {
                    // If someone else created it simultaneously, just reload
                    cart = await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating cart for user {UserId}", userId);
                    return Result.Failure<ShoppingCart>(DomainErrors.General.ServerError);
                }

                cart ??= await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
            }

            return cart != null 
                ? Result.Success(cart) 
                : Result.Failure<ShoppingCart>(DomainErrors.General.ServerError);
        }
    }
}
