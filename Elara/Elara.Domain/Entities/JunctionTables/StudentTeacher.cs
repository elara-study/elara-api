using Elara.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.JunctionTables
{
    public class StudentTeacher : BaseEntity
    {
        // Foreign Keys
        public int StudentId { get; set; }
        public int TeacherId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; } = null!;

        // Additional Properties
        public bool IsActive { get; set; } = true;

    }
}
