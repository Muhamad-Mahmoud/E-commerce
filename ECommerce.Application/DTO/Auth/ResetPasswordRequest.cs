using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTO.Auth
{
    /// <summary>
    /// Reset password request DTO.
    /// </summary>
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Confirm new password is required")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
