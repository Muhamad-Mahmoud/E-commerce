using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for shopping cart operations.
    /// </summary>
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        /// <summary>
        /// Gets a shopping cart by user ID with all items and product variants.
        /// </summary>
        Task<ShoppingCart?> GetByUserIdAsync(string userId);
    }
}
