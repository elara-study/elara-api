using FluentValidation;

namespace Elara.Application.Features.Notifications.Commands.UpdatePreferences
{
    public class UpdatePreferencesCommandValidator : AbstractValidator<UpdatePreferencesCommand>
    {
        public UpdatePreferencesCommandValidator()
        {
            RuleFor(n => n.UserId).NotEqual(Guid.Empty).WithMessage("UserId is required");
        }
    }
}
