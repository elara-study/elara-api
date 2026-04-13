using Elara.Domain.Entities.Administrative;

namespace Elara.Application.Contracts.Persistence
{
    public interface IPasswordResetOtpRepository
    {
        Task AddAsync(PasswordResetOtp otp, CancellationToken cancellationToken = default);
        Task<PasswordResetOtp?> GetValidOtpAsync(Guid userId, CancellationToken cancellationToken = default);
        Task InvalidateUserOtpsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}