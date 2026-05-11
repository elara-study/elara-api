using System.ComponentModel.DataAnnotations;

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
    }
}
