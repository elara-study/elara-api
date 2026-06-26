using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Educational
{
    public class Roadmap:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public GradeLevel Grade { get; set; }
        public string? Description { get; set; }

        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
    }
}
