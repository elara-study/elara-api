using Elara.Application.Contracts.Identity;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IIdentityService _identityService;

        public LogoutCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var result = await _identityService.RevokeRefreshTokenAsync(request.RefreshToken);
            if (!result)
            {
                throw new UnauthorizedAccessException("Token is already invalid or logged out.");
            }
        }
    }
}
