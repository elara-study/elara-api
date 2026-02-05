using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.JunctionTables
{
    public class StudentClass : BaseEntity
    {
        // Foreign Keys
        public Guid StudentId { get; set; }
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
