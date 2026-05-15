namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetStudentInsightsForTeacher
{
    public class StudentInsightForTeacherDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public List<ReportItemDto> Reports { get; set; } = [];
    }

    public class ReportItemDto
    {
        public Guid ReportId { get; set; }
        public Guid ConversationId { get; set; }
        public string ReportText { get; set; } = string.Empty;
        public int AnalyzedMessageCount { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }
}
