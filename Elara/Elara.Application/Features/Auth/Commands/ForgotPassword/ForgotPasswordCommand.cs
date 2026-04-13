using MediatR;

namespace Elara.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
    {
        public string Email { get; set; } = string.Empty;
    }
}