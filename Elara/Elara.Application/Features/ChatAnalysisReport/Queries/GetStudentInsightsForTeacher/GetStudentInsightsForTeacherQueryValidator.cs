using FluentValidation;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetStudentInsightsForTeacher
{
    public class GetStudentInsightsForTeacherQueryValidator
        : AbstractValidator<GetStudentInsightsForTeacherQuery>
    {
        public GetStudentInsightsForTeacherQueryValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty();
        }
    }
}
