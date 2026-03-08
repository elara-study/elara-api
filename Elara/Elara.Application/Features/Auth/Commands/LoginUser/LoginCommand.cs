using Elara.Application.Models.Auth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.LoginUser
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
