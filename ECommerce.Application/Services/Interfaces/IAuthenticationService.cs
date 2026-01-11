using ECommerce.Application.DTO;

namespace ECommerce.Application.Services.Interfaces
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
