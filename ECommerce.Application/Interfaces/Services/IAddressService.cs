using ECommerce.Application.DTO.Addresses.Requests;
using ECommerce.Application.DTO.Addresses.Responses;
using ECommerce.Domain.Shared;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IAddressService
    {
        Task<Result<IEnumerable<AddressDto>>> GetUserAddressesAsync(string userId);
        Task<Result<AddressDto>> GetByIdAsync(int id, string userId);
        Task<Result<AddressDto>> CreateAsync(string userId, CreateAddressDto request, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(int id, string userId, CancellationToken cancellationToken);
    }
}
