using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;

namespace Elara.Domain.Entities.Users
{
    public class Teacher : BaseEntity<Guid>
    {
        // Foreign Key
        public int? SubjectId { get; set; }

        // Navigation Properties
        public virtual Subject? Subject { get; set; }
        public virtual ICollection<StudentTeacher> StudentTeachers { get; set; } = [];
        public virtual ICollection<Class> Classes { get; set; } = [];
        public virtual ICollection<ProblemSet> ProblemSets { get; set; } = [];
        public virtual ICollection<Roadmap> Roadmaps { get; set; } = [];
    }
}
