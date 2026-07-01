using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetParentNotifications
{
    public class GetParentNotificationsQuery : IRequest<ParentNotificationsResponseDto>
    {
        public Guid UserId { get; }
        public int Page { get; }
        public int Limit { get; }

        public GetParentNotificationsQuery(Guid userId, int page, int limit)
        {
            UserId = userId;
            Page = page;
            Limit = limit;
        }
    }
}
