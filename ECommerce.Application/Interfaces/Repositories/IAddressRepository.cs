using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<IEnumerable<Address>> GetUserAddressesAsync(string userId);
    }
}
