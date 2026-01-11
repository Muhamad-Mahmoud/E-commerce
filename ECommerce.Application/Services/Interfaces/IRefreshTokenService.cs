using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshTokenAsync(string userId);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<bool> RevokeRefreshTokenAsync(string token);
        Task RevokeAllUserTokensAsync(string userId);
    }
}
