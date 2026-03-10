using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthUserData>
    {
        private readonly IIdentityService _identityService;

        public RegisterUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<AuthUserData> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.RegisterAsync(request.Email, request.Name, request.DateOfBirth, request.Password);
        }
    }
}
