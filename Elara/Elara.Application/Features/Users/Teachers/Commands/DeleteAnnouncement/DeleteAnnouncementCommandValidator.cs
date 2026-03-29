using FluentValidation;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteAnnouncement
{
    public class DeleteAnnouncementCommandValidator : AbstractValidator<DeleteAnnouncementCommand>
    {
        public DeleteAnnouncementCommandValidator()
        {
            RuleFor(x => x.ClassId)
                .NotEmpty().WithMessage("Class ID is required.");

            RuleFor(x => x.AnnouncementId)
                .NotEmpty().WithMessage("Announcement ID is required.");
        }
    }
}
