using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Infrastructure.UnitOfWork
{

    public class unitOfWork(AppDbContext context) : IUnitOfWork
    {
        private readonly AppDbContext _context = context;
        private IDbContextTransaction? _transaction;

        private ICategoryRepository? _categories;
        private IProductRepository? _products;
        private IOrderRepository? _orders;
        private IRefreshTokenRepository? _refreshTokens;

        //  Repositories

        public ICategoryRepository Categories
            => _categories ??= new CategoryRepository(_context);

        public IProductRepository Products
            => _products ??= new ProductRepository(_context);

        public IOrderRepository Orders
            => _orders ??= new OrderRepository(_context);

        public IRefreshTokenRepository RefreshTokens
            => _refreshTokens ??= new RefreshTokenRepository(_context);

        private IShoppingCartRepository? _shoppingCarts;
        public IShoppingCartRepository ShoppingCarts
            => _shoppingCarts ??= new ShoppingCartRepository(_context);

        //  Transaction Management 

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                return; // Transaction already started

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            try
            {
                await SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                return; // Nothing to rollback

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        //  Dispose 
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
