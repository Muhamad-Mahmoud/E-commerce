using System.Security.Cryptography;
using AutoMapper;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    /// <summary>
    /// Service for managing orders, including creation, status updates, and retrieval.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order for the specified user based on their current shopping cart.
        /// </summary>
        public async Task<Result<OrderResponse>> CreateOrderAsync(string userId, int shippingAddressId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure<OrderResponse>(DomainErrors.User.IdRequired);

            var address = await _unitOfWork.Addresses.GetByIdAsync(shippingAddressId);
            if (address == null || address.UserId != userId)
            {
                _logger.LogWarning("Address {AddressId} not found or for other user during order creation", shippingAddressId);
                return Result.Failure<OrderResponse>(DomainErrors.Order.ShippingAddressRequired);
            }

            var shippingAddress = new ShippingAddress
            {
                FullName = address.FullName,
                Phone = address.Phone,
                Country = address.Country,
                City = address.City,
                Street = address.Street,
                PostalCode = address.PostalCode
            };

            var cart = await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                _logger.LogWarning("User {UserId} attempted to create order with empty cart", userId);
                return Result.Failure<OrderResponse>(DomainErrors.Order.EmptyCart);
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = new Order(userId, GenerateOrderNumber(), shippingAddress);

                ProcessOrderItems(cart.Items, order);

                await _unitOfWork.Orders.AddAsync(order);
                _unitOfWork.ShoppingCarts.Delete(cart);
                
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Order {OrderNumber} created for user {UserId}", order.OrderNumber, userId);
                return Result.Success(_mapper.Map<OrderResponse>(order)!);
            }
            catch (InsufficientStockException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning("Order creation failed due to insufficient stock: {Message}", ex.Message);
                return Result.Failure<OrderResponse>(DomainErrors.Order.InsufficientStock);
            }
            catch (ConcurrencyConflictException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning(ex, "Concurrency conflict occurred during order creation for user {UserId}", userId);
                return Result.Failure<OrderResponse>(DomainErrors.Order.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating order for user {UserId}. Transaction rolled back.", userId);
                return Result.Failure<OrderResponse>(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Gets an order by its ID, ensuring it belongs to the specified user.
        /// </summary>
        public async Task<Result<OrderResponse>> GetOrderByIdAsync(int id, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure<OrderResponse>(DomainErrors.User.IdRequired);

            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", id);
                return Result.Failure<OrderResponse>(DomainErrors.Order.NotFound);
            }

            if (order.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted unauthorized access to order {OrderId}", userId, id);
                return Result.Failure<OrderResponse>(DomainErrors.Order.Unauthorized);
            }

            return Result.Success(_mapper.Map<OrderResponse>(order)!);
        }

        /// <summary>
        /// Gets all orders for a specific user with pagination support.
        /// </summary>
        public async Task<Result<PagedResult<OrderResponse>>> GetUserOrdersAsync(string userId, OrderParams orderParams)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure<PagedResult<OrderResponse>>(DomainErrors.User.IdRequired);

            return await SearchOrdersAsync(orderParams, userId);
        }

        /// <summary>
        /// Updates the status of an existing order.
        /// </summary>
        public async Task<Result<OrderResponse>> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Attempt to update status for non-existent order {OrderId}", id);
                return Result.Failure<OrderResponse>(DomainErrors.Order.NotFound);
            }

            try
            {
                order.UpdateStatus(status);
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} status changed to {NewStatus}", id, status);
                return Result.Success(_mapper.Map<OrderResponse>(order)!);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid status transition attempt for order {OrderId} to {NewStatus}: {Message}",
                    id, status, ex.Message);
                return Result.Failure<OrderResponse>(DomainErrors.Order.InvalidStatusTransition);
            }
        }

        /// <summary>
        /// Cancels an order, ensuring it belongs to the user and is in a cancellable state.
        /// Restores stock for items in the cancelled order.
        /// </summary>
        public async Task<Result<OrderResponse>> CancelOrderAsync(int orderId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure<OrderResponse>(DomainErrors.User.IdRequired);

            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for cancellation", orderId);
                return Result.Failure<OrderResponse>(DomainErrors.Order.NotFound);
            }

            if (order.UserId != userId)
            {
                _logger.LogWarning("User {UserId} unauthorized to cancel order {OrderId}", userId, orderId);
                return Result.Failure<OrderResponse>(DomainErrors.Order.Unauthorized);
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                order.Cancel();

                foreach (var item in order.OrderItems)
                {
                    var variant = await _unitOfWork.ProductVariants.GetByIdAsync(item.ProductVariantId);
                    if (variant != null)
                    {
                        variant.RestoreStock(item.Quantity);
                        _unitOfWork.ProductVariants.Update(variant);
                    }
                }

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Order {OrderNumber} cancelled by user {UserId}", order.OrderNumber, userId);
                return Result.Success(_mapper.Map<OrderResponse>(order)!);
            }
            catch (InvalidOperationException)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogWarning("Invalid cancellation attempt for order {OrderId} in state {Status}", orderId, order.Status);
                return Result.Failure<OrderResponse>(DomainErrors.Order.InvalidStatusTransition);
            }
            catch (ConcurrencyConflictException)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure<OrderResponse>(DomainErrors.Order.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error cancelling order {OrderId} for user {UserId}", orderId, userId);
                return Result.Failure<OrderResponse>(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Searches for orders based on specific parameters, optionally filtering by user.
        /// </summary>
        public async Task<Result<PagedResult<OrderResponse>>> SearchOrdersAsync(OrderParams orderParams, string? userId = null)
        {
            var pagedOrders = await _unitOfWork.Orders.SearchOrdersAsync(orderParams, userId);

            var result = new PagedResult<OrderResponse>(
                pagedOrders.PageNumber,
                pagedOrders.PageSize,
                pagedOrders.TotalCount,
                _mapper.Map<List<OrderResponse>>(pagedOrders.Items) ?? new List<OrderResponse>());

            return Result.Success(result);
        }

        private void ProcessOrderItems(IEnumerable<ShoppingCartItem> cartItems, Order order)
        {
            foreach (var cartItem in cartItems)
            {
                var variant = cartItem.ProductVariant;
                
                try 
                {
                    variant.DeductStock(cartItem.Quantity);
                }
                catch (InvalidOperationException)
                {
                    throw new InsufficientStockException(variant.Product.Name, variant.StockQuantity, cartItem.Quantity);
                }

                _unitOfWork.ProductVariants.Update(variant);

                order.AddItem(variant, cartItem.Quantity);
            }
        }

        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var randomNumber = RandomNumberGenerator.GetInt32(1000, 10000); // Thread-safe & secure
            return $"ORD-{timestamp}-{randomNumber}";
        }
    }
}
