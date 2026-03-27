using FluentValidation;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroupOverview
{
    public class GetStudentGroupOverviewQueryValidator : AbstractValidator<GetStudentGroupOverviewQuery>
    {
        public GetStudentGroupOverviewQueryValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEqual(Guid.Empty)
                .WithMessage("Student ID is required.");

            RuleFor(x => x.GroupId)
                .NotEqual(Guid.Empty)
                .WithMessage("Group ID must be a valid UUID.");
        }
    }
}
