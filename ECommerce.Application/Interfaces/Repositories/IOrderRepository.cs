using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository for Order operations.
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        // Order-specific query methods
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task<Order?> GetByOrderNumberAsync(string orderNumber);
        
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
        
        // Order-specific command
        Task<bool> OrderNumberExistsAsync(string orderNumber);
    }
}
