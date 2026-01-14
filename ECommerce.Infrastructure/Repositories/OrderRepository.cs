using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await GetByIdAsync(id,
                o => o.OrderItems,
                o => o.PaymentTransactions);
        }

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await GetFirstAsync(
                o => o.OrderNumber == orderNumber,
                o => o.OrderItems,
                o => o.PaymentTransactions);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await FindAsync(
                o => o.UserId == userId,
                o => o.OrderItems,
                o => o.PaymentTransactions);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await FindAsync(
                o => o.Status == status,
                o => o.OrderItems,
                o => o.PaymentTransactions);
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Include(o => o.PaymentTransactions)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> OrderNumberExistsAsync(string orderNumber)
        {
            return await _context.Orders
                .AsNoTracking()
                .AnyAsync(o => o.OrderNumber == orderNumber);
        }
    }
}