using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetSingleChildInsights
{
    public class GetSingleChildInsightsQuery : IRequest<SingleChildInsightDto>
    {
        public Guid ChildId { get; set; }
    }
}
