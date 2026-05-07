using FluentValidation;

namespace Elara.Application.Features.Notifications.Commands.RegisterDeviceToken
{
    public class RegisterDeviceTokenCommandValidator : AbstractValidator<RegisterDeviceTokenCommand>
    {
        public RegisterDeviceTokenCommandValidator()
        {
            RuleFor(n => n.UserId).NotEqual(Guid.Empty).WithMessage("UserId is required");

            RuleFor(n => n.Token).NotEmpty().WithMessage("Token is required")
                .MaximumLength(512).WithMessage("Token cannot exceed 512 characters");
        }
    }
}
