using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.Educational
{
    public class Topic : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? ModuleName { get; set; }

        // Foreign Key
        public int SubjectId { get; set; }
        
        public int RoadmapId { get; set; }

        // Navigation Property
        public virtual Subject Subject { get; set; } = null!;
        public virtual Roadmap Roadmap { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
