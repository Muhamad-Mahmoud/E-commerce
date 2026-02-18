using ECommerce.Application.DTO.Addresses.Requests;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// User addresses management.
    /// </summary>
    [Authorize]
    public class AddressesController : BaseApiController
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>
        /// Get current user's addresses.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            return Ok(await _addressService.GetUserAddressesAsync(UserId));
        }

        /// <summary>
        /// Get specific address by id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var address = await _addressService.GetByIdAsync(id, UserId);
            return address == null ? NotFound() : Ok(address);
        }

        /// <summary>
        /// Create new address.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAddressDto request, CancellationToken cancellationToken)
        {
            var address = await _addressService.CreateAsync(UserId, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = address.Id }, address);
        }

        /// <summary>
        /// Delete address.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _addressService.DeleteAsync(id, UserId, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}
