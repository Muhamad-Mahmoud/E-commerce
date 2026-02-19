using AutoMapper;
using ECommerce.Application.DTO.Addresses.Requests;
using ECommerce.Application.DTO.Addresses.Responses;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Shared;

namespace ECommerce.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddressService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<AddressDto>>> GetUserAddressesAsync(string userId)
        {
            var addresses = await _unitOfWork.Addresses.GetUserAddressesAsync(userId);
            return Result.Success(_mapper.Map<IEnumerable<AddressDto>>(addresses));
        }

        public async Task<Result<AddressDto>> GetByIdAsync(int id, string userId)
        {
            var address = await _unitOfWork.Addresses.GetFirstAsync(a => a.Id == id && a.UserId == userId);
            if (address == null)
                return Result.Failure<AddressDto>(DomainErrors.Address.NotFound);

            return Result.Success(_mapper.Map<AddressDto>(address));
        }

        public async Task<Result<AddressDto>> CreateAsync(string userId, CreateAddressDto request, CancellationToken cancellationToken)
        {
            var address = _mapper.Map<Address>(request);
            address.UserId = userId;

            if (address.IsDefaultShipping)
            {
                var existingDefaults = await _unitOfWork.Addresses.FindAsync(
                    a => a.UserId == userId && a.IsDefaultShipping);

                foreach (var addr in existingDefaults)
                {
                    addr.IsDefaultShipping = false;
                    _unitOfWork.Addresses.Update(addr);
                }
            }

            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(_mapper.Map<AddressDto>(address));
        }

        public async Task<Result> DeleteAsync(int id, string userId, CancellationToken cancellationToken)
        {
            var address = await _unitOfWork.Addresses.GetFirstAsync(a => a.Id == id && a.UserId == userId);
            if (address == null)
                return Result.Failure(DomainErrors.Address.NotFound);

            _unitOfWork.Addresses.Delete(address);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
