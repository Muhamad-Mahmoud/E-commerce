using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern for managing transactions and coordinating repositories.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Gets the category repository for accessing category data.
        /// </summary>
        ICategoryRepository Categories { get; }

        /// <summary>
        /// Gets the product repository for accessing product data.
        /// </summary>
        IProductRepository Products { get; }

        /// <summary>
        /// Gets the order repository for accessing order data.
        /// </summary>
        IOrderRepository Orders { get; }

        /// <summary>
        /// Gets the refresh token repository for accessing token data.
        /// </summary>
        IRefreshTokenRepository RefreshTokens { get; }

        /// <summary>
        /// Gets the shopping cart repository for accessing shopping cart data.
        /// </summary>
        IShoppingCartRepository ShoppingCarts { get; }

        /// <summary>
        /// Gets the product variant repository for managing stock and variant data.
        /// </summary>
        IProductVariantRepository ProductVariants { get; }

        /// <summary>
        /// Gets the address repository for managing user addresses.
        /// </summary>
        IAddressRepository Addresses { get; }

        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        /// <returns>The number of entities written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current database transaction and saves all changes.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no active transaction exists.</exception>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current database transaction, discarding all changes.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RollbackTransactionAsync();
    }
}
