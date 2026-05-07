using Elara.Domain.Enums;

namespace Elara.Application.Features.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public NotificationType Type { get; set; }
        public DateTime NotificationDate { get; set; }
    }
}
