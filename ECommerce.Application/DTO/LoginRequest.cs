using System.ComponentModel.DataAnnotations;
namespace ECommerce.Application.DTO
{
    /// <summary>
    /// Login request DTO.
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
