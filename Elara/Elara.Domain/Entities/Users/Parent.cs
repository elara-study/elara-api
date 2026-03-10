namespace Elara.Domain.Entities.Users
{
    public class Parent : BaseEntity<Guid>
    {
        // Navigation Properties
        public virtual ICollection<Student> Childrens { get; set; } = [];
    }
}
