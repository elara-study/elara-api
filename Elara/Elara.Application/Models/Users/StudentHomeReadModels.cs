using Elara.Domain.Enums;

namespace Elara.Application.Models.Users
{
    public class LatestQuizSessionReadModel
    {
        public int? ModuleId { get; set; }
        public string? ModuleTitle { get; set; }
    }

    public class TodayQuizSessionReadModel
    {
        public QuizSessionStatus Status { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int UnansweredCount { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class EnrolledClassReadModel
    {
        public Guid PublicId { get; set; }
        public string ClassName { get; set; } = string.Empty;
    }
}
