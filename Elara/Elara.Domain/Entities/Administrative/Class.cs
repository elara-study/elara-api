using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Administrative
{
    public class Class : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public GradeLevel Level { get; set; } 


        // Navigation properties
        public virtual ICollection<StudentClass> StudentClasses { get; set; }
        public virtual ICollection<ClassTeacher> ClassTeachers { get; set; } 

    }
}
