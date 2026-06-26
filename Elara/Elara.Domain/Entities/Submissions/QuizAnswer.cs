using Elara.Domain.Enums;

namespace Elara.Domain.Entities.Submissions
{
    public class QuizAnswer : BaseEntity
    {
        public string QuestionText { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }

        // The answer the student submitted
        public string StudentAnswer { get; set; } = string.Empty;

        // The correct answer (for MCQ: option text; for Essay: reference answer)
        public string CorrectAnswer { get; set; } = string.Empty;

        public bool? IsCorrect { get; set; }

        public int XpAwarded { get; set; } = 0;
        public bool HintUsed { get; set; } = false;

        public int QuizSessionId { get; set; }

        public virtual QuizSession QuizSession { get; set; } = null!;
    }
}
