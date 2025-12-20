using Elara.Domain.Entities.Educational;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.JunctionTables
{
    public class LessonVideo : BaseEntity
    {
        // Foreign Keys
        public int LessonId { get; set; }
        public int VideoId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(LessonId))]
        public virtual Lesson Lesson { get; set; } = null!;

        [ForeignKey(nameof(VideoId))]
        public virtual EducationalVideo Video { get; set; } = null!;
    }
}
