using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IIdentityService _identityService;

        public ResetPasswordCommandHandler(
            IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = await _identityService.GetUserIdByEmailAsync(request.Email);

            var isValid = await _identityService.VerifyPasswordResetOtpAsync(userId, request.Otp);
            if (!isValid)
                throw new InvalidOperationException("Invalid or expired OTP.");

            await _identityService.ResetPasswordWithOtpAsync(userId, request.NewPassword);
        }
    }
}