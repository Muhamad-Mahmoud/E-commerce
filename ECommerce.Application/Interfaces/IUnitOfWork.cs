using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern for managing transactions and coordinating repositories.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IRefreshTokenRepository RefreshTokens { get; }

        // Transaction management
        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
