using ECommerce.Application.DTO.Auth.Requests;
using ECommerce.Application.DTO.Auth.Responses;

namespace ECommerce.Application.Interfaces.Services.Auth
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> RegisterAsync(RegisterRequest request);

        Task<AuthenticationResponse> LoginAsync(LoginRequest request);


        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);

        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
        Task<AuthenticationResponse> RefreshTokenAsync(string token);
    }
}

