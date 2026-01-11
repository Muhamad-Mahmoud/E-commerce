using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string email);
    }
}
