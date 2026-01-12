using ECommerce.Application.DTO;
using ECommerce.Application.DTO.Auth;

namespace ECommerce.Application.Interfaces.Services.Auth
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> RegisterAsync(RegisterRequest request);
        Task<AuthenticationResult> LoginAsync(LoginRequest request);
        Task LogoutAsync(string userId);
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
        Task<AuthenticationResult> RefreshTokenAsync(string token);
    }
}

