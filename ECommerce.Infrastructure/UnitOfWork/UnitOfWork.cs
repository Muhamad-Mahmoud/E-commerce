using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.UnitOfWork
{
    public class unitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        private ICategoryRepository? _categories;
        private IProductRepository? _products;
        private IOrderRepository? _orders;
        private IRefreshTokenRepository? _refreshTokens;
        private IShoppingCartRepository? _shoppingCarts;
        private IProductVariantRepository? _productVariants;
        private IAddressRepository? _addresses;

        public unitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public ICategoryRepository Categories
            => _categories ??= new CategoryRepository(_context);

        public IProductRepository Products
            => _products ??= new ProductRepository(_context);

        public IOrderRepository Orders
            => _orders ??= new OrderRepository(_context);

        public IRefreshTokenRepository RefreshTokens
            => _refreshTokens ??= new RefreshTokenRepository(_context);

        public IShoppingCartRepository ShoppingCarts
            => _shoppingCarts ??= new ShoppingCartRepository(_context);

        public IProductVariantRepository ProductVariants
            => _productVariants ??= new ProductVariantRepository(_context);

        public IAddressRepository Addresses
            => _addresses ??= new AddressRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                return;

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
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                return;

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

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
