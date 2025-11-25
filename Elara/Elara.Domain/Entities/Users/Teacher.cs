using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Users
{
    public class Teacher : User
    {

        // Foreign Key
        public int? SubjectId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(SubjectId))]
        public virtual Subject? Subject { get; set; }

        public virtual ICollection<StudentTeacher> StudentTeachers { get; set; } = new List<StudentTeacher>();
        public virtual ICollection<ClassTeacher> ClassTeachers { get; set; } = new List<ClassTeacher>();
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
