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
        public string StudentId { get; set; }
        public string TeacherId { get; set; }

        // Navigation Properties
       
        public virtual Student Student { get; set; } = null!;

        public virtual Teacher Teacher { get; set; } = null!;

    }
}
