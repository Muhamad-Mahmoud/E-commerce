using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        IProductVariantRepository ProductVariants { get; }
        IAddressRepository Addresses { get; }
        IWishlistRepository Wishlists { get; }
        IReviewRepository Reviews { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
