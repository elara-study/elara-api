using Elara.Domain.Entities.Administrative;

namespace Elara.Application.Contracts.Persistence
{
    public interface IOtpRepository
    {
        Task AddAsync(OtpCode otp, CancellationToken cancellationToken = default);
        Task<OtpCode?> GetValidOtpAsync(Guid userId, OtpType type, CancellationToken cancellationToken = default);
        Task InvalidateUserOtpsAsync(Guid userId, OtpType type, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}