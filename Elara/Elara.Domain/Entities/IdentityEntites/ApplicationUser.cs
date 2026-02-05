using Elara.Domain.Entities.Administrative;
using Microsoft.AspNetCore.Identity;
namespace Elara.Domain.Entities.IdentityEntites
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Navigation properties
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
