namespace ECommerce.Application.DTO.Auth.Responses
{
    public class UserResponse
    {
        public string Identifier { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
