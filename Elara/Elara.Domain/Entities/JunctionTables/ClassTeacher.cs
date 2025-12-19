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
    public class ClassTeacher : BaseEntity
    {
        // Foreign Keys
        public int ClassId { get; set; }
        public string TeacherId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(ClassId))]
        public virtual Class Class { get; set; } = null!;

        public virtual Teacher Teacher { get; set; } = null!;

    }
}
