using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Auth.Requests
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string Token { get; set; } = string.Empty;
    }
}
