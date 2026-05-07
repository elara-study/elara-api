using MediatR;

namespace Elara.Application.Features.Notifications.Queries.GetUnreadCount
{
    public class GetUnreadCountQuery : IRequest<int>
    {
        public Guid UserId { get; }

        public GetUnreadCountQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
