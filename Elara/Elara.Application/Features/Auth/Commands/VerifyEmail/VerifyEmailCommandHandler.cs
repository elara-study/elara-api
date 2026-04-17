using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
    {
        private readonly IIdentityService _identityService;

        public VerifyEmailCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var userId = await _identityService.GetUserIdByEmailAsync(request.Email);

            var isValid = await _identityService.VerifyEmailOtpAsync(userId, request.Otp);
            if (!isValid)
                throw new InvalidOperationException("Invalid or expired OTP.");

            await _identityService.ConfirmEmailAsync(userId);
        }
    }
}