using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Administrative
{
    public class Notification : BaseEntity
    {
        [Required]
        public Guid PublicId { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;
        public NotificationType NotificationType { get; set; }
        public DateTime NotificationDate { get; set; } = DateTime.UtcNow;
        

        // Foreign Key
        [Required]
        public Guid UserId { get; set; }
    }
}
