namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetAllReports
{
    public class GetReportsDto
    {
        public Guid ReportId { get; set; }
        public Guid ConversationId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string ReportText { get; set; } = string.Empty;
        public int AnalyzedMessageCount { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }
}
