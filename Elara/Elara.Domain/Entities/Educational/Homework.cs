using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Educational
{
    public class Homework : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public int EstimatedDurationMinutes { get; set; } = 30;

        public string? Description { get; set; }
        public DateTime DueDate { get; set; }

        [Range(0, 1000)]
        public int MaxScore { get; set; } = 100;
        public bool IsRequired { get; set; } = true;
        public bool IsAIGenerated { get; set; } = false;
        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Easy;

        public int ModuleId { get; set; }

        public virtual Module Module { get; set; } = null!;

        public virtual ICollection<HomeworkVideo> HomeworkVideos { get; set; }
        public virtual ICollection<Problem> Problems { get; set; }

        public bool IsValidDueDate()
        {
            return DueDate > CreatedAt;
        }
    }
}
