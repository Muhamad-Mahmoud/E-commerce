using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository for Order operations.
    /// </summary>
    public interface IOrderRepository
    {
        // Query by business needs
        Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default);
        
        // Commands
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default);
        
        // Persistence
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
