using Elara.Application.Contracts.persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Notifications.Queries.GetNotifications
{
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetNotificationsQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetByUserId(
                request.UserId, request.Page, request.Limit, cancellationToken);

            return notifications.Select(n => new NotificationDto
            {
                Id = n.PublicId,
                Message = n.Message,
                IsRead = n.IsRead,
                Type = n.NotificationType,
                NotificationDate = n.NotificationDate
            }).ToList();
        }
    }
}
