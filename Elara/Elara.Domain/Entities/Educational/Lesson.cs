using Elara.Domain.Entities.JunctionTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Educational
{
    public class Lesson : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string? Content { get; set; }

        public int EstimatedDurationMinutes { get; set; } = 30;

        // Foreign Key
        [Required]
        public int TopicId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(TopicId))]
        public virtual Topic Topic { get; set; } = null!;

        public virtual ICollection<LessonVideo> LessonVideos { get; set; } = new List<LessonVideo>();
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
