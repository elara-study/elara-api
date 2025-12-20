using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

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
