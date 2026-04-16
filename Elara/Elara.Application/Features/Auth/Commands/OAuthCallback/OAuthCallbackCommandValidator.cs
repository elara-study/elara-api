using FluentValidation;

namespace Elara.Application.Features.Auth.Commands.OAuthCallback
{
    public class OAuthCallbackCommandValidator : AbstractValidator<OAuthCallbackCommand>
    {
        public OAuthCallbackCommandValidator()
        {
            RuleFor(x => x.Provider)
                .NotEmpty().WithMessage("Provider is required.");

            RuleFor(x => x.ProviderUserId)
                .NotEmpty().WithMessage("ProviderUserId is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
