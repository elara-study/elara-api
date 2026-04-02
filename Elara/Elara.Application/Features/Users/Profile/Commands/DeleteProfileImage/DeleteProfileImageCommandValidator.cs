using FluentValidation;

namespace Elara.Application.Features.Users.Profile.Commands.DeleteProfileImage
{
    public class DeleteProfileImageCommandValidator : AbstractValidator<DeleteProfileImageCommand>
    {
        public DeleteProfileImageCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}
