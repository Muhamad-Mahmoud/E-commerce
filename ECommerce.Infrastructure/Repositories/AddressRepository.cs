using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Address>> GetUserAddressesAsync(string userId)
        {
            return await _dbSet
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }
    }
}
