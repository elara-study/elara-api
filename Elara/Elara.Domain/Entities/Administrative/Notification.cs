using Elara.Domain.Entities.IdentityEntites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.Administrative
{
    public class Notification : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;
        public DateTime NotificationDate { get; set; } = DateTime.UtcNow;


        // Foreign Key
        [Required]
        public Guid UserId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
