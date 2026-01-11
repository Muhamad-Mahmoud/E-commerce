namespace ECommerce.Infrastructure.Persistence
{
    public record UserSeedDto(string Username, string Email, string FullName, string Role, bool ResetPassword);
}
