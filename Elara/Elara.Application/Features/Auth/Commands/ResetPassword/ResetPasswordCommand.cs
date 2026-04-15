using MediatR;

namespace Elara.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
