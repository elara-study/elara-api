using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.JunctionTables
{
    public class StudentClass : BaseEntity
    {
        // Foreign Keys
        public string StudentId { get; set; }
        public int ClassId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey(nameof(ClassId))]
        public virtual Class Class { get; set; } = null!;

        // Additional Properties
        public bool IsActive { get; set; } = true;
        public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;

    }
}
