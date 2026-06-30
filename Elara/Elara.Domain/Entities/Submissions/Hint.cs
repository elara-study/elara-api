using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Submissions
{
    public class Hint : BaseEntity
    {
        public string Content { get; set; } = string.Empty;

        public bool IsAIGenerated { get; set; } = false;

        [Range(1, 5)]
        public int HintLevel { get; set; } = 1; // 1 = subtle, 5 = very explicit

        // Foreign Keys
        public int ProblemId { get; set; }
        public Guid StudentId { get; set; }

        // Navigation Properties
        public virtual Problem Problem { get; set; } = null!;
        public virtual Student Student { get; set; }
    }
}
