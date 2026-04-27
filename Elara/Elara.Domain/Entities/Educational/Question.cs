using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Educational
{
    public class Question : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Easy;
        public bool IsAIGenerated { get; set; } = false;
        public bool HasVideoSupport { get; set; } = false;

        [Range(0, 100)]
        public double Marks { get; set; } = 10;

        // Foreign Key
        public int AssignmentId { get; set; }

        // Navigation Properties
        public virtual Assignment Assignment { get; set; } = null!;
        public virtual ICollection<Hint> Hints { get; set; } = [];

        // Options for MCQ questions only (empty for Essay questions)
        public virtual ICollection<QuestionOption> Options { get; set; } = [];
    }
}
