using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IOtpRepository _otpRepository;

        public VerifyEmailCommandHandler(IIdentityService identityService,IOtpRepository otpRepository)
        {
            _identityService = identityService;
            _otpRepository = otpRepository;
        }

        public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var userId = await _identityService.GetUserIdByEmailAsync(request.Email);

            var otpEntity = await _otpRepository.GetValidOtpAsync(userId, OtpType.EmailVerification, cancellationToken);
            if (otpEntity == null)
                throw new InvalidOperationException("OTP has expired or does not exist.");

            if (otpEntity.Attempts >= 3)
                throw new InvalidOperationException("Maximum attempts exceeded. Please request a new OTP.");

            var otpHash = ComputeHash(request.Otp);
            if (otpEntity.OtpHash != otpHash)
            {
                otpEntity.Attempts++;
                await _otpRepository.SaveChangesAsync(cancellationToken);
                throw new InvalidOperationException("Invalid OTP.");
            }

            otpEntity.IsUsed = true;
            await _otpRepository.SaveChangesAsync(cancellationToken);

            await _identityService.ConfirmEmailAsync(userId);
        }

        private static string ComputeHash(string value)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            var hash = System.Security.Cryptography.SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}