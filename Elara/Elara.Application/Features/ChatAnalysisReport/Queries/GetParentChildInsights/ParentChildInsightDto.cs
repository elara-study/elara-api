namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetParentChildInsights
{
    public class ParentChildInsightDto
    {
        public Guid ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public List<ParentReportItemDto> Reports { get; set; } = [];
    }

    public class ParentReportItemDto
    {
        public Guid ReportId { get; set; }
        public Guid ConversationId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string ReportText { get; set; } = string.Empty;
        public int AnalyzedMessageCount { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }
}
