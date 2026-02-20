using ECommerce.Application.DTO.Auth.Requests;
using ECommerce.Application.DTO.Auth.Responses;
using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Interfaces.Services.Auth
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request);
        Task<Result> ChangePasswordAsync(ChangePasswordRequest request);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
        Task<Result<AuthenticationResponse>> RefreshTokenAsync(string token);
    }
}

