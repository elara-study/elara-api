using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Educational
{
    public class ProblemOption : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;

        // Foreign Key
        public int ProblemId { get; set; }

        // Navigation Property
        public virtual Problem Problem { get; set; } = null!;
    }
}
