using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Services.Auth
{
    /// <summary>
    /// Interface for refresh token management operations.
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Generates a new refresh token for a user.
        /// </summary>
        Task<RefreshToken> GenerateRefreshTokenAsync(string userId);

        /// <summary>
        /// Retrieves a refresh token by its token value.
        /// </summary>
        Task<RefreshToken?> GetByTokenAsync(string token);

        /// <summary>
        /// Revokes a specific refresh token (marks as used, doesn't delete for audit trail).
        /// </summary>
        Task<bool> RevokeRefreshTokenAsync(string token);

        /// <summary>
        /// Revokes all active refresh tokens for a specific user.
        /// </summary>
        Task RevokeAllUserTokensAsync(string userId);
    }
}
