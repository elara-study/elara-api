using FluentValidation;
using Elara.Domain.Enums;

namespace Elara.Application.Features.Auth.Commands.CompleteOAuthRegistration
{
    public class CompleteOAuthRegistrationCommandValidator : AbstractValidator<CompleteOAuthRegistrationCommand>
    {
        private static readonly string[] AllowedRoles = ["student", "teacher"];

        public CompleteOAuthRegistrationCommandValidator()
        {
            RuleFor(x => x.Provider)
                .NotEmpty().WithMessage("Provider is required.");

            RuleFor(x => x.ProviderUserId)
                .NotEmpty().WithMessage("ProviderUserId is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(r => AllowedRoles.Contains(r.Trim().ToLower()))
                .WithMessage("Role must be 'student' or 'teacher'.");

            RuleFor(x => x.SubjectId)
                .NotNull().WithMessage("SubjectId is required for teachers.")
                .GreaterThan(0).WithMessage("SubjectId must be a valid ID.")
                .When(x => string.Equals(x.Role?.Trim(), "teacher", StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.Grade)
                .NotNull().WithMessage("Grade is required for students.")
                .Must(grade => Enum.IsDefined(typeof(GradeLevel), grade!.Value))
                .WithMessage("Invalid grade level. Valid grades are 10, 11, or 12.")
                .When(x => string.Equals(x.Role?.Trim(), "student", StringComparison.OrdinalIgnoreCase));
        }
    }
}
