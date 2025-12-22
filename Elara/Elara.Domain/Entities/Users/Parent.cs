namespace Elara.Domain.Entities.Users
{
    public class Parent : BaseEntity
    {
        public string ParentId { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual ICollection<Student> Childrens { get; set; }
    }
}
