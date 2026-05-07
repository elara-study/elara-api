using FluentValidation;

namespace Elara.Application.Features.Notifications.Commands.RemoveDeviceToken
{
    public class RemoveDeviceTokenCommandValidator : AbstractValidator<RemoveDeviceTokenCommand>
    {
        public RemoveDeviceTokenCommandValidator()
        {
            RuleFor(n => n.Token).NotEmpty().WithMessage("Token is required");
        }
    }
}
