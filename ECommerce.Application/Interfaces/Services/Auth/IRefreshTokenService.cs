using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Services.Auth
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshTokenAsync(string userId);

        Task<RefreshToken?> GetByTokenAsync(string token);

        Task<bool> RevokeRefreshTokenAsync(string token);

        Task RevokeAllUserTokensAsync(string userId);
    }
}
