using MediatR;

namespace Elara.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommand : IRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}