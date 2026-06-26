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

        // AI ephemeral quiz — serialized JSON of questions
        public string? QuestionsJson { get; set; }

        public string? ElaraInsight { get; set; }

        public Guid StudentId { get; set; }

        // For teacher-created problem sets
        public int? ProblemSetId { get; set; }

        // For AI-generated quizzes scoped to a module
        public int? ModuleId { get; set; }

        public virtual Student Student { get; set; } = null!;
        public virtual ProblemSet? ProblemSet { get; set; }
        public virtual Module? Module { get; set; }
        public virtual ICollection<QuizAnswer> Answers { get; set; } = [];
    }
}
