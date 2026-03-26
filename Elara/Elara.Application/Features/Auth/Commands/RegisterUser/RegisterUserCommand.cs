using Elara.Application.Models.Auth;
using MediatR;

namespace Elara.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserCommand : IRequest<AuthUserData>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Role { get; set; } = string.Empty;
        public int? SubjectId { get; set; }
    }
}

