using FluentValidation;

namespace Elara.Application.Features.Users.Teachers.Queries.GetGroupStudents
{
    public class GetGroupStudentsQueryValidator : AbstractValidator<GetGroupStudentsQuery>
    {
        public GetGroupStudentsQueryValidator()
        {
            RuleFor(x => x.ClassId)
                .NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }
}
