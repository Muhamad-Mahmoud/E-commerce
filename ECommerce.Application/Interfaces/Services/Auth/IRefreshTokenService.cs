using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Services.Auth
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshTokenAsync(string userId);
        Task<RefreshToken?> GetByTokenAsync(string token);
        /// <summary>
        /// Marks the token as revoked without deleting it (for security audit).
        /// </summary>
        Task<bool> RevokeRefreshTokenAsync(string token);
        Task RevokeAllUserTokensAsync(string userId);
    }
}
