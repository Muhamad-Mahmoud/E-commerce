using AutoMapper;
using ECommerce.Application.DTO.Addresses.Requests;
using ECommerce.Application.DTO.Addresses.Responses;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AddressService> _logger;
        public AddressService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddressService> _logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this._logger = _logger;
        }

        /// <summary>
        /// Gets all addresses for a specific user.
        /// </summary>
        public async Task<Result<IEnumerable<AddressDto>>> GetUserAddressesAsync(string userId)
        {
            var addresses = await _unitOfWork.Addresses.GetUserAddressesAsync(userId);
            return Result.Success(_mapper.Map<IEnumerable<AddressDto>>(addresses) ?? Enumerable.Empty<AddressDto>());
        }

        /// <summary>
        /// Gets a specific address by its ID for a given user.
        /// </summary>
        public async Task<Result<AddressDto>> GetByIdAsync(int id, string userId)
        {
            var address = await _unitOfWork.Addresses.GetFirstAsync(a => a.Id == id && a.UserId == userId);
            if (address == null)
                return Result.Failure<AddressDto>(DomainErrors.Address.NotFound);

            return Result.Success(_mapper.Map<AddressDto>(address)!);
        }

        /// <summary>
        /// Creates a new address for a user.
        /// </summary>
        public async Task<Result<AddressDto>> CreateAsync(string userId, CreateAddressDto request, CancellationToken cancellationToken)
        {
            try 
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

                _logger.LogInformation("New address {AddressId} created for user {UserId}", address.Id, userId);
                return Result.Success(_mapper.Map<AddressDto>(address)!);
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict during address creation for user {UserId}", userId);
                return Result.Failure<AddressDto>(DomainErrors.Order.ConcurrencyConflict); // Reusing Order concurrency error as it's generic enough or should have Address.ConcurrencyConflict
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address for user {UserId}", userId);
                return Result.Failure<AddressDto>(DomainErrors.General.ServerError);
            }
        }

        /// <summary>
        /// Deletes an address for a user.
        /// </summary>
        public async Task<Result> DeleteAsync(int id, string userId, CancellationToken cancellationToken)
        {
            var address = await _unitOfWork.Addresses.GetFirstAsync(a => a.Id == id && a.UserId == userId);
            if (address == null)
            {
                _logger.LogWarning("Address {AddressId} not found for deletion by user {UserId}", id, userId);
                return Result.Failure(DomainErrors.Address.NotFound);
            }

            try 
            {
                _unitOfWork.Addresses.Delete(address);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Address {AddressId} deleted for user {UserId}", id, userId);
                return Result.Success();
            }
            catch (ConcurrencyConflictException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict during address deletion for user {UserId}, address {AddressId}", userId, id);
                return Result.Failure(DomainErrors.Order.ConcurrencyConflict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address {AddressId} for user {UserId}", id, userId);
                return Result.Failure(DomainErrors.General.ServerError);
            }
        }
    }
}
