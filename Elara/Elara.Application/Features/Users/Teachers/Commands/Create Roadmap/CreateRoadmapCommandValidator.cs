using FluentValidation;

namespace Elara.Application.Features.Users.Teachers.Commands.Create_Roadmap
{
    public class CreateRoadmapCommandValidator : AbstractValidator<CreateRoadmapCommand>
    {
        public CreateRoadmapCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Roadmap name is required.")
                .MinimumLength(5).WithMessage("Roadmap name must be at least 5 characters.")
                .MaximumLength(100).WithMessage("Roadmap name cannot exceed 100 characters.");

            RuleFor(x => x.Grade)
                .Must(BeValidGrade).WithMessage("Grade must be 10, 11, or 12.");

            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("SubjectId must be a positive integer.");
        }

        private bool BeValidGrade(int grade) => grade == 10 || grade == 11 || grade == 12;
    }
}
