using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Educational
{
    public class QuestionOption : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;

        // Foreign Key
        public int QuestionId { get; set; }

        // Navigation Property
        public virtual Question Question { get; set; } = null!;
    }
}
