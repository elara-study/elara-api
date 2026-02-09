using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;
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

        // Navigation properties
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int? RoadmapId { get; set; } 
        public Roadmap? Roadmap { get; set; }

        public virtual ICollection<StudentClass> StudentClasses { get; set; }
        public virtual ICollection<ClassTeacher> ClassTeachers { get; set; } 

    }
}
