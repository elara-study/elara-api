using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetParentChildInsights
{
    public class GetParentChildInsightsQuery : IRequest<List<ParentChildInsightDto>>
    {
    }
}
