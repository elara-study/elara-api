using Elara.Domain.Entities.JunctionTables;
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
        public string Level { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
        public virtual ICollection<ClassTeacher> ClassTeachers { get; set; } = new List<ClassTeacher>();

    }
}
