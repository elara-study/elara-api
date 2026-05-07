using MediatR;

namespace Elara.Application.Features.Notifications.Commands.MarkBatchAsRead
{
    public class MarkBatchAsReadCommand : IRequest
    {
        public List<Guid> NotificationIds { get; }
        public Guid UserId { get; }

        public MarkBatchAsReadCommand(List<Guid> notificationIds, Guid userId)
        {
            NotificationIds = notificationIds;
            UserId = userId;
        }
    }
}
