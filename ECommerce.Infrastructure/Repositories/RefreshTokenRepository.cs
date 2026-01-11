using ECommerce.Domain.Entities;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.Set<RefreshToken>()
                .SingleOrDefaultAsync(r => r.Token == token);
        }

        public async Task<List<RefreshToken>> GetAllActiveTokensByUserIdAsync(string userId)
        {
            return await _context.Set<RefreshToken>()
                .Where(t => t.UserId == userId && t.RevokedOn == null)
                .ToListAsync();
        }
    }
}
