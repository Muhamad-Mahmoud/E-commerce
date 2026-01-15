namespace ECommerce.Application.DTO.Auth.Responses
{
    /// <summary>
    /// Result model for authentication operations.
    /// </summary>
    public class AuthenticationResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserResponse? User { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}
