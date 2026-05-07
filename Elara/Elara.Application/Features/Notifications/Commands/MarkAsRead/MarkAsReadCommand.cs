using MediatR;

namespace Elara.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkAsReadCommand : IRequest
    {
        public Guid NotificationId { get; }
        public Guid UserId { get; }

        public MarkAsReadCommand(Guid notificationId, Guid userId)
        {
            NotificationId = notificationId;
            UserId = userId;
        }
    }
}
