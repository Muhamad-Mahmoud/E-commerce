using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    /// <summary>
    /// Order repository implementation.
    /// </summary>
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => _context.OrderItems.Where(oi => oi.OrderId == id))
                .Include(o => _context.PaymentTransactions.Where(pt => pt.OrderId == id))
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AnyAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        }
    }
}

