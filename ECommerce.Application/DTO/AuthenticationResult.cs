namespace ECommerce.Application.DTO
{
    /// <summary>
    /// Result model for authentication operations.
    /// </summary>
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}
