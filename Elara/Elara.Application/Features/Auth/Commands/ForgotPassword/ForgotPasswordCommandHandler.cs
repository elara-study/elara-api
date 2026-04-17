using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        public ForgotPasswordCommandHandler(
            IIdentityService identityService,
            IEmailService emailService)
        {
            _identityService = identityService;
            _emailService = emailService;
        }
        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = await _identityService.GetUserIdByEmailAsync(request.Email);

            var otp = await _identityService.GeneratePasswordResetOtpAsync(userId);

            await _emailService.SendPasswordResetEmailAsync(request.Email, request.Email, otp, cancellationToken);

            return new ForgotPasswordResponse { Message = "OTP sent successfully." };
        }
    }
}