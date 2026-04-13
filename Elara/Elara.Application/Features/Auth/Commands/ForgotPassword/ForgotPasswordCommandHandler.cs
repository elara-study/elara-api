using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IPasswordResetOtpRepository _otpRepository;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            IIdentityService identityService,
            IPasswordResetOtpRepository otpRepository,
            IEmailService emailService)
        {
            _identityService = identityService;
            _otpRepository = otpRepository;
            _emailService = emailService;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = await _identityService.GetUserIdByEmailAsync(request.Email);

            await _otpRepository.InvalidateUserOtpsAsync(userId, cancellationToken);

            var otp = GenerateOtp();
            var otpHash = ComputeHash(otp);

            await _otpRepository.AddAsync(new PasswordResetOtp
            {
                UserId = userId,
                OtpHash = otpHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                Attempts = 0,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _otpRepository.SaveChangesAsync(cancellationToken);

            await _emailService.SendPasswordResetEmailAsync(request.Email, request.Email, otp, cancellationToken);

            return new ForgotPasswordResponse { Message = "OTP sent successfully." };
        }

        private static string GenerateOtp() =>
            Random.Shared.Next(100000, 999999).ToString();

        private static string ComputeHash(string value)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            var hash = System.Security.Cryptography.SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}