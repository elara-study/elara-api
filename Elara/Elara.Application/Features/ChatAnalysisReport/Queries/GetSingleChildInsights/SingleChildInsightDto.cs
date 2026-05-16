using Elara.Application.Features.ChatAnalysisReport.Queries.GetParentChildInsights;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetSingleChildInsights
{
    public class SingleChildInsightDto
    {
        public Guid ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public List<ParentReportItemDto> Reports { get; set; } = [];
    }
}
