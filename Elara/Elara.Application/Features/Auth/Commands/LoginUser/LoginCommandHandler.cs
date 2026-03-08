using Elara.Application.Contracts.Identity;
using Elara.Application.Models.Auth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.LoginUser
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // TODO: replace with real DB lookup once users are persisted
            const string knownEmail = "teacher@elara.study";
            const string knownPassword = "Password123";
            const string fullName = "Test Teacher";
            var teacherId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

            if (!string.Equals(request.Email, knownEmail, StringComparison.OrdinalIgnoreCase)
                || request.Password != knownPassword)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var token = _tokenService.CreateToken(teacherId, knownEmail, fullName, "Teacher");

            return Task.FromResult(new LoginResponse { Token = token });
        }
    }
}
