using FluentValidation;
using Elara.Application.Common.Validation;

namespace Elara.Application.Features.Users.Profile.Commands.UpdateProfileImage
{
    public class UpdateProfileImageCommandValidator : AbstractValidator<UpdateProfileImageCommand>
    {
        public UpdateProfileImageCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.FileBytes)
                .NotNull().WithMessage("Image content is required.")
                .Must(x => x.Length > 0).WithMessage("Image content is required.")
                .Must(x => x.Length <= ProfileImageValidation.MaxImageSizeBytes).WithMessage("Image file size must not exceed 5MB.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content type is required.")
                .Must(ProfileImageValidation.IsAllowedContentType).WithMessage("Only JPG, PNG, and WebP images are allowed.");

            RuleFor(x => x)
                .Must(x => ProfileImageValidation.HasValidSignature(x.FileBytes, x.ContentType))
                .WithMessage("Invalid image content. The uploaded file does not match the declared format.");
        }
    }
}
