using ECommerce.Application.Interfaces.Services.Auth;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Helper;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ECommerce.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JWT _jwtSettings;

        public RefreshTokenService(IUnitOfWork unitOfWork, IOptions<JWT> jwtSettings)
        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(string userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);

            if (refreshToken == null || !refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task RevokeAllUserTokensAsync(string userId)
        {
            var userTokens = await _unitOfWork.RefreshTokens.GetAllActiveTokensByUserIdAsync(userId);

            if (userTokens.Any())
            {
                foreach (var token in userTokens)
                {
                    token.RevokedOn = DateTime.UtcNow;
                }
                _unitOfWork.RefreshTokens.UpdateRange(userTokens);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

