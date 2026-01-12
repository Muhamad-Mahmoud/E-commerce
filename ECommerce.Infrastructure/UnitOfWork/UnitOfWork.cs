using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Unit of Work implementation for managing database transactions.
    /// Provides coordinated access to multiple repositories and transaction management.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy initialization of repositories
        private ICategoryRepository? _categories;
        private IProductRepository? _products;
        private IOrderRepository? _orders;
        private IRefreshTokenRepository? _refreshTokens;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // ============ Repositories (Lazy Initialization) ============

        public ICategoryRepository Categories
            => _categories ??= new CategoryRepository(_context);

        public IProductRepository Products
            => _products ??= new ProductRepository(_context);

        public IOrderRepository Orders
            => _orders ??= new OrderRepository(_context);

        public IRefreshTokenRepository RefreshTokens
            => _refreshTokens ??= new RefreshTokenRepository(_context);

        // ============ Transaction Management ============

        /// <summary>
        /// Saves all pending changes to the database.
        /// For single SaveChanges, EF Core uses implicit transactions automatically.
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Begins an explicit database transaction.
        /// Use transactions only when multiple SaveChanges or complex operations are involved.
        /// EF Core handles implicit transactions for single SaveChanges automatically.
        /// </summary>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                return; // Transaction already started

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Commits the current transaction and saves all changes.
        /// Automatically rolls back if an error occurs.
        /// </summary>
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            try
            {
                await SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// Safe to call even if no transaction is active.
        /// </summary>
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                return; // Nothing to rollback

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // ============ Dispose ============

        /// <summary>
        /// Disposes the Unit of Work and releases database resources.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
