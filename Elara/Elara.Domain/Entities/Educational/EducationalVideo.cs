using Elara.Domain.Entities.JunctionTables;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Educational
{
    public class EducationalVideo : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Url]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int DurationInSeconds { get; set; } = 0;


        // Navigation Properties
        public virtual ICollection<LessonVideo> LessonVideos { get; set; } = new List<LessonVideo>();
    }
}
