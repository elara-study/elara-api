namespace Elara.Domain.Entities.Users
{
    public class Parent : User
    {
        public string ParentId { get; set; }
        // Navigation Properties
        public virtual User parent { get; set; }

        public virtual ICollection<Student> Childrens { get; set; }
    }
}
