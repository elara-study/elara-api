using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.LoginUser
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.ValidateUserCredentialsAsync(request.Email, request.Password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var token = _tokenService.CreateToken(user);
            var refresh = await _identityService.GenerateRefreshTokenAsync(user.UserId);

            return new LoginResponse { Token = token, RefreshToken = refresh };
        }
    }
}
