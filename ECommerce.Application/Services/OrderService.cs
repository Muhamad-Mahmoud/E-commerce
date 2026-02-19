using System.Security.Cryptography;
using AutoMapper;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderResponse>> CreateOrderAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure<OrderResponse>(DomainErrors.User.IdRequired);

            var cart = await _unitOfWork.ShoppingCarts.GetByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                _logger.LogWarning("User {UserId} attempted to create order with empty cart", userId);
                return Result.Failure<OrderResponse>(DomainErrors.Order.EmptyCart);
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    OrderNumber = GenerateOrderNumber(),
                    Status = OrderStatus.Pending,
                    PaymentStatus = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };

                ProcessOrderItems(cart.Items, order);

                order.TotalAmount = order.OrderItems.Sum(i => i.ItemTotal);

                await _unitOfWork.Orders.AddAsync(order);
                _unitOfWork.ShoppingCarts.Delete(cart);
                
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Order {OrderNumber} created for user {UserId}", order.OrderNumber, userId);
                return Result.Success(_mapper.Map<OrderResponse>(order));
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
                throw;
            }
        }

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
                _logger.LogWarning("User {UserId} attempted unauthorized access to order {OrderId} belonging to user {OrderOwnerId}",
                    userId, id, order.UserId);
                return Result.Failure<OrderResponse>(DomainErrors.Order.Unauthorized);
            }

            return Result.Success(_mapper.Map<OrderResponse>(order));
        }

        public async Task<Result<IEnumerable<OrderResponse>>> GetUserOrdersAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure<IEnumerable<OrderResponse>>(DomainErrors.User.IdRequired);

            var orders = await _unitOfWork.Orders.GetUserOrdersAsync(userId);
            return Result.Success(_mapper.Map<IEnumerable<OrderResponse>>(orders));
        }

        public async Task<Result<OrderResponse>> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Attempt to update status for non-existent order {OrderId}", id);
                return Result.Failure<OrderResponse>(DomainErrors.Order.NotFound);
            }

            if (!IsValidStatusTransition(order.Status, status))
            {
                _logger.LogWarning("Invalid status transition attempt for order {OrderId} from {CurrentStatus} to {NewStatus}",
                    id, order.Status, status);
                return Result.Failure<OrderResponse>(DomainErrors.Order.InvalidStatusTransition);
            }

            var previousStatus = order.Status;
            order.Status = status;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} status changed from {PreviousStatus} to {NewStatus}",
                id, previousStatus, status);
            return Result.Success(_mapper.Map<OrderResponse>(order));
        }

        public async Task<Result<PagedResult<OrderResponse>>> SearchOrdersAsync(OrderParams orderParams, string? userId = null)
        {
            var pagedOrders = await _unitOfWork.Orders.SearchOrdersAsync(orderParams, userId);

            var result = new PagedResult<OrderResponse>
            {
                Items = _mapper.Map<List<OrderResponse>>(pagedOrders.Items),
                TotalCount = pagedOrders.TotalCount,
                PageNumber = pagedOrders.PageNumber,
                PageSize = pagedOrders.PageSize
            };

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

                var orderItem = new OrderItem
                {
                    ProductVariantId = cartItem.ProductVariantId,
                    ProductName = $"{variant.Product.Name} - {variant.SKU}",
                    UnitPrice = variant.Price,
                    Quantity = cartItem.Quantity,
                    ItemTotal = variant.Price * cartItem.Quantity
                };
                order.OrderItems.Add(orderItem);
            }
        }

        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var randomNumber = RandomNumberGenerator.GetInt32(1000, 10000); // Thread-safe & secure
            return $"ORD-{timestamp}-{randomNumber}";
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending => newStatus is OrderStatus.Processing or OrderStatus.Cancelled,
                OrderStatus.Processing => newStatus is OrderStatus.Shipped or OrderStatus.Cancelled,
                OrderStatus.Shipped => newStatus is OrderStatus.Delivered,
                OrderStatus.Delivered => false,
                OrderStatus.Cancelled => false,
                _ => false
            };
        }
    }
}
