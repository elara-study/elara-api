namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetConversationReport
{
    public class ConversationReportDto
    {
        public Guid ReportId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ReportText { get; set; } = string.Empty;
        public int AnalyzedMessageCount { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }
}
