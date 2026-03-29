using FluentValidation;

namespace Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement
{
    public class AddAnnouncementCommandValidator : AbstractValidator<AddAnnouncementCommand>
    {
        public AddAnnouncementCommandValidator()
        {
            RuleFor(x => x.ClassId)
                .NotEmpty().WithMessage("Class ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(5).WithMessage("Title must be at least 5 characters.")
                .MaximumLength(50).WithMessage("Title cannot exceed 50 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MinimumLength(10).WithMessage("Content must be at least 10 characters.")
                .MaximumLength(500).WithMessage("Content cannot exceed 500 characters.");
        }
    }
}
