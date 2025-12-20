using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.Educational
{
    public class Question : BaseEntity
    {
        [Required]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        public QuestionType QuestionType { get; set; }

        [Required]
        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Easy;

        public bool IsAIGenerated { get; set; } = false;

        public bool HasVideoSupport { get; set; } = false;

        [Range(0, 100)]
        public double Marks { get; set; } = 10;

        // Foreign Key
        [Required]
        public int AssignmentId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(AssignmentId))]
        public virtual Assignment Assignment { get; set; } = null!;

        public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();
    }
}
