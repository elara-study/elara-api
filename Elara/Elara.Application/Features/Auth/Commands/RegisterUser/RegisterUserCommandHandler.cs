using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthUserData>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public RegisterUserCommandHandler(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<AuthUserData> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var registeredUser = await _identityService.RegisterAsync(new RegisterUserData
            {
                Email = request.Email,
                Password = request.Password,
                Name = request.Name,
                DateOfBirth = request.DateOfBirth
            });

            registeredUser.Token = _tokenService.CreateToken(registeredUser);

            return registeredUser;
        }
    }
}
