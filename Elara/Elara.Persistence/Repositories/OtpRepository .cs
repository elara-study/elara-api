using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly AppDbContext _context;
        public OtpRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(OtpCode otp, CancellationToken cancellationToken = default)
        {
            await _context.OtpCodes.AddAsync(otp, cancellationToken);
        }
        public async Task<OtpCode?> GetValidOtpAsync(Guid userId, OtpType type, CancellationToken cancellationToken = default)
        {
            return await _context.OtpCodes
                .Where(x => x.UserId == userId
                         && x.Type == type
                         && !x.IsUsed
                         && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task InvalidateUserOtpsAsync(Guid userId, OtpType type, CancellationToken cancellationToken = default)
        {
            await _context.OtpCodes
                .Where(x => x.UserId == userId && x.Type == type && !x.IsUsed)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsUsed, true), cancellationToken);
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}