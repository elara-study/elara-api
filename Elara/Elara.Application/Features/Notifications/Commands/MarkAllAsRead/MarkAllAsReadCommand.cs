using MediatR;

namespace Elara.Application.Features.Notifications.Commands.MarkAllAsRead
{
    public class MarkAllAsReadCommand : IRequest
    {
        public Guid UserId { get; }

        public MarkAllAsReadCommand(Guid userId)
        {
            UserId = userId;
        }
    }
}
