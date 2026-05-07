using MediatR;

namespace Elara.Application.Features.Notifications.Queries.GetNotifications
{
    public class GetNotificationsQuery : IRequest<List<NotificationDto>>
    {
        public Guid UserId { get; }
        public int Page { get; }
        public int Limit { get; }

        public GetNotificationsQuery(Guid userId, int page, int limit)
        {
            UserId = userId;
            Page = page;
            Limit = limit;
        }
    }
}
