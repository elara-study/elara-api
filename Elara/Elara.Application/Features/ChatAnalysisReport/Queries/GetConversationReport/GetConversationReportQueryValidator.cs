using FluentValidation;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetConversationReport
{
    public class GetConversationReportQueryValidator
        : AbstractValidator<GetConversationReportQuery>
    {
        public GetConversationReportQueryValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty();
        }
    }
}
