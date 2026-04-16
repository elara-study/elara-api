using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using Elara.Application.Models.OAuth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.CompleteOAuthRegistration
{
    public class CompleteOAuthRegistrationCommandHandler : IRequestHandler<CompleteOAuthRegistrationCommand, LoginResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService    _tokenService;

        public CompleteOAuthRegistrationCommandHandler(IIdentityService identityService,ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService    = tokenService;
        }

        public async Task<LoginResponse> Handle(CompleteOAuthRegistrationCommand request, CancellationToken cancellationToken)
        {
            var completeData = new CompleteOAuthData
            {
                Provider       = request.Provider,
                ProviderUserId = request.ProviderUserId,
                Email          = request.Email,
                Name           = request.Name,
                Role           = request.Role,
                SubjectId      = request.SubjectId,
                DateOfBirth    = request.DateOfBirth
            };

            var user    = await _identityService.CompleteOAuthRegistrationAsync(completeData);
            var token   = _tokenService.CreateToken(user);
            var refresh = await _identityService.GenerateRefreshTokenAsync(user.UserId);

            return new LoginResponse { Token = token, RefreshToken = refresh };
        }
    }
}
