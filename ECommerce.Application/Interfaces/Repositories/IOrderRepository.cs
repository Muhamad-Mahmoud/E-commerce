using ECommerce.Application.DTO.Pagination;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Order operations.
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Gets an order by ID with all related details including items and payment transactions.
        /// </summary>
        Task<Order?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Gets an order by its order number with all related details.
        /// </summary>
        Task<Order?> GetByOrderNumberAsync(string orderNumber);

        /// <summary>
        /// Gets all orders for a specific user, ordered by creation date.
        /// </summary>
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);

        /// <summary>
        /// Gets all orders with a specific status, ordered by creation date.
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);

        /// <summary>
        /// Gets the most recent orders up to a specified count.
        /// </summary>
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);

        /// <summary>
        /// Checks if an order with the given order number exists.
        /// </summary>
        Task<bool> OrderNumberExistsAsync(string orderNumber);

        /// <summary>
        /// Searches for orders with pagination, filtering, and sorting.
        /// </summary>
        Task<PagedResult<Order>> SearchOrdersAsync(OrderParams orderParams, string? userId = null);
    }
}
