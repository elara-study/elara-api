using FluentValidation;

namespace Elara.Application.Features.Users.Students.Commands.JoinGroup
{
    public class JoinGroupCommandValidator : AbstractValidator<JoinGroupCommand>
    {
        public JoinGroupCommandValidator()
        {
            RuleFor(x => x.JoinCode)
                .NotEmpty().WithMessage("Join code is required.")
                .MaximumLength(20).WithMessage("Join code cannot exceed 20 characters.");
        }
    }
}
