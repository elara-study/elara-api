using Elara.Domain.Entities.Administrative;
using Microsoft.AspNetCore.Identity;

namespace Elara.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; } = [];
        public virtual ICollection<Report> Reports { get; set; } = [];
    }
}
