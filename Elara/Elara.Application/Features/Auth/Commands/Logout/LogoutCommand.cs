using MediatR;

namespace Elara.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest
    {
        public string RefreshToken { get; set; } = string.Empty;

        public LogoutCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
