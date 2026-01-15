namespace ECommerce.Application.Interfaces.Services.Auth
{
    /// <summary>
    /// Interface for JWT token generation and management.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT access token for a user with specified roles.
        /// </summary>
        string GenerateAccessToken(string userId, string email, IList<string> roles);
    }
}
