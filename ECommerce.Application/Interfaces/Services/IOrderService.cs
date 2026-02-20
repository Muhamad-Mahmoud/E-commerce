using ECommerce.Domain.Exceptions;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Result<OrderResponse>> CreateOrderAsync(string userId, int shippingAddressId);

        Task<Result<OrderResponse>> GetOrderByIdAsync(int id, string userId);

        Task<Result<IEnumerable<OrderResponse>>> GetUserOrdersAsync(string userId);

        Task<Result<OrderResponse>> UpdateOrderStatusAsync(int id, OrderStatus status);

        Task<Result<OrderResponse>> CancelOrderAsync(int orderId, string userId);

        Task<Result<PagedResult<OrderResponse>>> SearchOrdersAsync(OrderParams orderParams, string? userId = null);
    }
}

