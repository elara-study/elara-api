using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, LoginResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public VerifyEmailCommandHandler(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var userId = await _identityService.GetUserIdByEmailAsync(request.Email);

            var isValid = await _identityService.VerifyEmailOtpAsync(userId, request.Otp);
            if (!isValid)
                throw new InvalidOperationException("Invalid or expired OTP.");

            await _identityService.ConfirmEmailAsync(userId);

            var user = await _identityService.GetUserByIdAsync(userId);
            var token = _tokenService.CreateToken(user);
            var refreshToken = await _identityService.GenerateRefreshTokenAsync(userId);

            return new LoginResponse { Token = token, RefreshToken = refreshToken };
        }
    }
}