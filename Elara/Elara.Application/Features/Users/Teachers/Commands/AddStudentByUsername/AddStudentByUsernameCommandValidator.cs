using FluentValidation;

namespace Elara.Application.Features.Users.Teachers.Commands.AddStudentByUsername
{
    public class AddStudentByUsernameCommandValidator : AbstractValidator<AddStudentByUsernameCommand>
    {
        public AddStudentByUsernameCommandValidator()
        {
            RuleFor(x => x.ClassId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }
}
