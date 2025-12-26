using Elara.Domain.Entities.IdentityEntites;

namespace Elara.Domain.Entities.Users
{
    public class Parent : BaseEntity
    {
        public Guid ParentId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Student> Childrens { get; set; }
    }
}
