using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Administrative
{
    public class Class : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        public GradeLevel Level { get; set; }

        [Required]
        [MaxLength(20)]
        public string JoinCode { get; set; } = string.Empty;

        // Foreign Keys
        public int SubjectId { get; set; }
        public Guid TeacherId { get; set; }
        public int? RoadmapId { get; set; }

        // Navigation properties
        public Educational.Subject Subject { get; set; }
        public Teacher Teacher { get; set; } = null!;
        public Roadmap? Roadmap { get; set; }
        public virtual ICollection<StudentClass> StudentClasses { get; set; }
    }
}
