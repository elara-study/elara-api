using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = await _identityService.GetUserByRefreshTokenAsync(request.RefreshToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var revoked = await _identityService.RevokeRefreshTokenAsync(request.RefreshToken);
            if (!revoked)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var token = _tokenService.CreateToken(user);
            var newRefresh = await _identityService.GenerateRefreshTokenAsync(user.UserId);

            return new LoginResponse { Token = token, RefreshToken = newRefresh };
        }
    }
}
