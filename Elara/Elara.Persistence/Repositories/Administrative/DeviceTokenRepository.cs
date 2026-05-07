using Elara.Application.Contracts.persistence.Administrative;
using Elara.Domain.Entities.Administrative;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories.Administrative
{
    public class DeviceTokenRepository : BaseRepository<DeviceToken, int>, IDeviceTokenRepository
    {
        public DeviceTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<string>> GetTokensByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
           var result = await _context.DeviceTokens.AsNoTracking()
                .Where(d=>d.UserId== userId).
                Select(d=>d.Token).ToListAsync(ct);
            return result;
           
        }

        public async Task RemoveByTokenAsync(string token, CancellationToken ct = default)
        {
            await _context.DeviceTokens.Where(d => d.Token == token)
                .ExecuteDeleteAsync(ct);
        }

        public async Task UpsertAsync(Guid userId, string token, CancellationToken ct = default)
        {
            var device = await _context.DeviceTokens
                .Where(d => d.UserId == userId && d.Token == token)
                .FirstOrDefaultAsync(ct);
            if (device == null)
            {
                await _context.DeviceTokens.AddAsync(new DeviceToken
                {
                    UserId = userId,
                    Token = token,
                    LastUsedAt = DateTime.UtcNow
                }, ct);
            }
            else
            {
                device.LastUsedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync(ct);
        }
    }
}
