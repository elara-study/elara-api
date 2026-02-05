using Elara.Domain.Entities.Users;

namespace Elara.Domain.Entities.JunctionTables
{
    public class StudentTeacher : BaseEntity
    {
        // Foreign Keys
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }

        // Navigation Properties
        public virtual Student Student { get; set; } = null!;
        public virtual Teacher Teacher { get; set; } = null!;

    }
}
