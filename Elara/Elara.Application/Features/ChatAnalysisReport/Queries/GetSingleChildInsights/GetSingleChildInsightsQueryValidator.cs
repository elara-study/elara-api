using FluentValidation;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetSingleChildInsights
{
    public class GetSingleChildInsightsQueryValidator
        : AbstractValidator<GetSingleChildInsightsQuery>
    {
        public GetSingleChildInsightsQueryValidator()
        {
            RuleFor(x => x.ChildId)
                .NotEmpty();
        }
    }
}
