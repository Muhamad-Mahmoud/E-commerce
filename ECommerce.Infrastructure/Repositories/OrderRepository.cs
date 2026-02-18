using ECommerce.Application.DTO.Pagination;
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
            return await ApplyDefaultIncludes(_context.Orders)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await ApplyDefaultIncludes(_context.Orders)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await ApplyDefaultIncludes(_context.Orders)
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await ApplyDefaultIncludes(_context.Orders)
                .AsNoTracking()
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count)
        {
            return await ApplyDefaultIncludes(_context.Orders)
                .AsNoTracking()
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

        public async Task<PagedResult<Order>> SearchOrdersAsync(OrderParams p, string? userId = null)
        {
            var query = ApplyDefaultIncludes(_context.Orders)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.UserId == userId);
            }

            if (!string.IsNullOrEmpty(p.Search))
            {
                var searchTerm = p.Search.ToLower();
                query = query.Where(o => o.OrderNumber.ToLower().Contains(searchTerm));
            }

            if (p.Status.HasValue)
            {
                query = query.Where(o => o.Status == p.Status.Value);
            }

            if (p.PaymentStatus.HasValue)
            {
                query = query.Where(o => o.PaymentStatus == p.PaymentStatus.Value);
            }

            if (p.FromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= p.FromDate.Value);
            }

            if (p.ToDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= p.ToDate.Value);
            }

            if (p.MinAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount >= p.MinAmount.Value);
            }

            if (p.MaxAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount <= p.MaxAmount.Value);
            }

            query = p.Sort switch
            {
                OrderParams.SortDateAsc => query.OrderBy(o => o.CreatedAt),
                OrderParams.SortDateDesc => query.OrderByDescending(o => o.CreatedAt),
                OrderParams.SortAmountAsc => query.OrderBy(o => o.TotalAmount),
                OrderParams.SortAmountDesc => query.OrderByDescending(o => o.TotalAmount),
                _ => query.OrderByDescending(o => o.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((p.PageNumber - 1) * p.PageSize)
                .Take(p.PageSize)
                .ToListAsync();

            return new PagedResult<Order>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = p.PageNumber,
                PageSize = p.PageSize
            };
        }

        private IQueryable<Order> ApplyDefaultIncludes(IQueryable<Order> query)
        {
            return query
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                .Include(o => o.PaymentTransactions);
        }
    }
}
