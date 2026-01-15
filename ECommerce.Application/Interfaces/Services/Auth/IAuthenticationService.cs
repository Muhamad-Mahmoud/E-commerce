using ECommerce.Application.DTO.Auth.Requests;
using ECommerce.Application.DTO.Auth.Responses;

namespace ECommerce.Application.Interfaces.Services.Auth
{
    /// <summary>
    /// Interface for user authentication and authorization service operations.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Registers a new user account with email and password.
        /// </summary>
        Task<AuthenticationResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Authenticates a user and returns JWT tokens.
        /// </summary>
        Task<AuthenticationResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Logs out a user by revoking their refresh tokens.
        /// </summary>
        Task LogoutAsync(string userId);

        /// <summary>
        /// Changes the password for a specific user.
        /// </summary>
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);

        /// <summary>
        /// Resets a user's password using a reset token.
        /// </summary>
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);

        /// <summary>
        /// Generates new JWT tokens using a valid refresh token.
        /// </summary>
        Task<AuthenticationResponse> RefreshTokenAsync(string token);
    }
}

