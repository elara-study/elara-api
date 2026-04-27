using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities.Submissions
{
    public class QuizSession : BaseEntity
    {
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public QuizSessionStatus Status { get; set; } = QuizSessionStatus.InProgress;

        public int XpEarned { get; set; } = 0;
        public int CorrectAnswers { get; set; } = 0;
        public int WrongAnswers { get; set; } = 0;
        public int UnansweredCount { get; set; } = 0;

        // AI insight
        public string? ElaraInsight { get; set; }
        public string? WeakTopics { get; set; }       
        public string? InsightRecommendation { get; set; }

        // Foreign Keys
        public Guid StudentId { get; set; }
        public int AssignmentId { get; set; }

        // Navigation Properties
        public virtual Student Student { get; set; } = null!;
        public virtual Assignment Assignment { get; set; } = null!;
        public virtual ICollection<QuizAnswer> Answers { get; set; } = [];
    }
}
