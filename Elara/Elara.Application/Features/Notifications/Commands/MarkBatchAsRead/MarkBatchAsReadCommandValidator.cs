using FluentValidation;

namespace Elara.Application.Features.Notifications.Commands.MarkBatchAsRead
{
    public class MarkBatchAsReadCommandValidator : AbstractValidator<MarkBatchAsReadCommand>
    {
        public MarkBatchAsReadCommandValidator()
        {
            RuleFor(n => n.UserId).NotEqual(Guid.Empty).WithMessage("UserId is required");

            RuleFor(n => n.NotificationIds).NotEmpty().WithMessage("NotificationIds list cannot be empty");

            RuleForEach(n => n.NotificationIds).NotEqual(Guid.Empty).WithMessage("NotificationId cannot be empty");
        }
    }
}
