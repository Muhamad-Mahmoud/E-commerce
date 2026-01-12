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
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
