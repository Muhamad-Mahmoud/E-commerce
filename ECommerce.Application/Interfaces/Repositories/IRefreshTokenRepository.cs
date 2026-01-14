using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for refresh token operations.
    /// </summary>
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        /// <summary>
        /// Gets a refresh token by its token value.
        /// </summary>
        Task<RefreshToken?> GetByTokenAsync(string token);

        /// <summary>
        /// Gets all active (non-revoked) refresh tokens for a specific user.
        /// </summary>
        Task<List<RefreshToken>> GetAllActiveTokensByUserIdAsync(string userId);
    }
}
