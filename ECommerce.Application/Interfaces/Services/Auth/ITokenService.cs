using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Services.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string email);
    }
}
