using FluentValidation;

namespace Elara.Application.Features.Chat.Commands.StartConversation
{
    public class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
    {
        public StartConversationCommandValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(2000).WithMessage("Message must not exceed 2000 characters.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(100).WithMessage("Subject must not exceed 100 characters.");
        }
    }
}
