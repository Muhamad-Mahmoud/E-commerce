using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Interfaces.Services
{
    /// <summary>
    /// Interface for order service operations.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Creates a new order from the user's shopping cart.
        /// </summary>
        /// <param name="userId">The user ID creating the order.</param>
        /// <returns>The created order response.</returns>
        Task<OrderResponse> CreateOrderAsync(string userId);

        /// <summary>
        /// Retrieves a specific order by ID with authorization check.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <param name="userId">The user ID requesting the order.</param>
        /// <returns>The order response.</returns>
        Task<OrderResponse> GetOrderByIdAsync(int id, string userId);

        /// <summary>
        /// Retrieves all orders for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of order responses.</returns>
        Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(string userId);

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <param name="status">The new order status.</param>
        /// <returns>The updated order response.</returns>
        Task<OrderResponse> UpdateOrderStatusAsync(int id, OrderStatus status);

        /// <summary>
        /// Searches for orders with pagination, filtering, and sorting.
        /// </summary>
        /// <param name="orderParams">Pagination and filtering parameters.</param>
        /// <param name="userId">Optional user ID to filter orders by user.</param>
        /// <returns>A paginated list of order responses.</returns>
        Task<PagedResult<OrderResponse>> SearchOrdersAsync(OrderParams orderParams, string? userId = null);
    }
}
