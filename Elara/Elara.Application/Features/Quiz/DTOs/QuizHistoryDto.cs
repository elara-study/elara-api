
namespace Elara.Application.Features.Quiz.DTOs
{
    public class QuizHistoryListDto
    {
        public List<QuizHistoryDto> Sessions { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class QuizHistoryDto
    {
        public int SessionId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public int XpEarned { get; set; }
        public double AccuracyPercentage { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
