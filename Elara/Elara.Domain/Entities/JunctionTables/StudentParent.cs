using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities.JunctionTables
{
    public class StudentParent : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid ParentId { get; set; }
        public StudentParentRelationStatus Status { get; set; } = StudentParentRelationStatus.Pending;
        public Guid? InitiatedById { get; set; }

        public virtual Student Student { get; set; } = null!;
        public virtual Parent Parent { get; set; } = null!;
    }
}
