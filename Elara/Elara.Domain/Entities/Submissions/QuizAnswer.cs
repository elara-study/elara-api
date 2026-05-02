using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities.Submissions
{
    public class QuizAnswer : BaseEntity
    {
        // type of question to identify it 
        public QuestionType QuestionType { get; set; }

        // for MCQ: the id of the option the student chose
        public int? SelectedOptionId { get; set; }

        // for Essay: the text the student wrote
        public string? AnswerContent { get; set; }

        // null = unanswered | true = correct | false = wrong
        public bool? IsCorrect { get; set; }

        public int XpAwarded { get; set; } = 0;
        public bool HintUsed { get; set; } = false;

        // Foreign Keys
        public int QuizSessionId { get; set; }
        public int QuestionId { get; set; }

        // Navigation Properties
        public virtual QuizSession QuizSession { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
        public virtual QuestionOption? SelectedOption { get; set; }
    }
}
