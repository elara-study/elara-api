using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using Elara.Application.Models.OAuth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.OAuthCallback
{
    public class OAuthCallbackCommandHandler : IRequestHandler<OAuthCallbackCommand, OAuthCallbackResponse>
    {
        private readonly IIdentityService    _identityService;
        private readonly ITokenService       _tokenService;
        private readonly IPendingTokenService _pendingTokenService;

        public OAuthCallbackCommandHandler(
            IIdentityService identityService,
            ITokenService tokenService,
            IPendingTokenService pendingTokenService)
        {
            _identityService    = identityService;
            _tokenService       = tokenService;
            _pendingTokenService = pendingTokenService;
        }

        public async Task<OAuthCallbackResponse> Handle(OAuthCallbackCommand request, CancellationToken cancellationToken)
        {
            var oauthData = new OAuthUserData
            {
                Provider       = request.Provider,
                ProviderUserId = request.ProviderUserId,
                Email          = request.Email
            };

            var existingUser = await _identityService.FindExistingOAuthUserAsync(oauthData);

            if (existingUser != null)
            {
                var token   = _tokenService.CreateToken(existingUser);
                var refresh = await _identityService.GenerateRefreshTokenAsync(existingUser.UserId);

                return new OAuthCallbackResponse
                {
                    IsPending     = false,
                    LoginResponse = new LoginResponse { Token = token, RefreshToken = refresh }
                };
            }

            return new OAuthCallbackResponse
            {
                IsPending    = true,
                PendingToken = _pendingTokenService.CreatePendingToken(request)
            };
        }
    }
}
