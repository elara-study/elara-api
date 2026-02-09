using Elara.Application.Features.Administrative.Classes.Commands.Create_Class;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Features.Users.Teachers.Commands.Create_Class
{
    public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
    {
        public CreateClassCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Class name is required.")
                .MaximumLength(100).WithMessage("Class name cannot exceed 100 characters.");

            RuleFor(x => x.Grade)
                .Must(BeValidGrade).WithMessage("Grade must be 10, 11, or 12.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(100).WithMessage("Subject name cannot exceed 100 characters.");

            RuleFor(x => x.RoadmapName)
                .MaximumLength(200).WithMessage("Roadmap name cannot exceed 200 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.RoadmapName));
        }

        private bool BeValidGrade(int grade)
        {
            return grade == 10 || grade == 11 || grade == 12;
        }
    }
}
