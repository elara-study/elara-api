using Elara.Domain.Entities.Administrative;
using Elara.Domain.Enums;
namespace Elara.Domain.Entities.Users
{
    public class User : BaseEntity
    {
        public DateTime? DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        // Navigation properties
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
