using Elara.Domain.Entities.JunctionTables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.Educational
{
    public class Lesson : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public int EstimatedDurationMinutes { get; set; } = 30;

        // Foreign Key
        public int TopicId { get; set; }

        // Navigation Properties
        public virtual Topic Topic { get; set; } = null!;

        public virtual ICollection<LessonVideo> LessonVideos { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; } 
    }
}
