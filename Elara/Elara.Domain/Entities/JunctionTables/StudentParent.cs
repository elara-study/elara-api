using Elara.Domain.Entities.Users;

namespace Elara.Domain.Entities.JunctionTables
{
    public class StudentParent : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid ParentId { get; set; }

        public virtual Student Student { get; set; } = null!;
        public virtual Parent Parent { get; set; } = null!;
    }
}
