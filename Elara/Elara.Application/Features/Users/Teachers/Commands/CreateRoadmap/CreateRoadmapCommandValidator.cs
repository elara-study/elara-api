using Elara.Domain.Enums;
using FluentValidation;

namespace Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap
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
                .Must(grade => Enum.IsDefined(typeof(GradeLevel), grade))
                .WithMessage("Grade must be one of the allowed domain grade levels.");
        }
    }
}
