using ECommerce.Application.DTO.Addresses.Requests;
using ECommerce.Application.DTO.Addresses.Responses;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetUserAddressesAsync(string userId);
        Task<AddressDto?> GetByIdAsync(int id, string userId);
        Task<AddressDto> CreateAsync(string userId, CreateAddressDto request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, string userId, CancellationToken cancellationToken);
    }
}
