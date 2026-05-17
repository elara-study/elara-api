using Elara.Domain.Entities.JunctionTables;

namespace Elara.Domain.Entities.Users
{
    public class Parent : BaseEntity<Guid>
    {
        public virtual ICollection<StudentParent> StudentParents { get; set; } = [];
    }
}
