using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories
{
    public class PasswordResetOtpRepository : IPasswordResetOtpRepository
    {
        private readonly AppDbContext _context;

        public PasswordResetOtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetOtp otp, CancellationToken cancellationToken = default)
        {
            await _context.PasswordResetOtps.AddAsync(otp, cancellationToken);
        }

        public async Task<PasswordResetOtp?> GetValidOtpAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.PasswordResetOtps
                .Where(x => x.UserId == userId
                         && !x.IsUsed
                         && x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task InvalidateUserOtpsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _context.PasswordResetOtps
                .Where(x => x.UserId == userId && !x.IsUsed)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsUsed, true), cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}