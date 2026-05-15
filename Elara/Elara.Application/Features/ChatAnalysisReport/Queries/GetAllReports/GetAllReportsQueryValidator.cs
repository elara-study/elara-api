using FluentValidation;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetAllReports
{
    public class GetAllReportsQueryValidator
        : AbstractValidator<GetAllReportsQuery>
    {
        public GetAllReportsQueryValidator()
        {
        }
    }
}
