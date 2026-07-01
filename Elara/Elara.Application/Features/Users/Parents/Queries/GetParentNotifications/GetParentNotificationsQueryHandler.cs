using Elara.Application.Contracts.persistence.Administrative;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetParentNotifications
{
    public class GetParentNotificationsQueryHandler : IRequestHandler<GetParentNotificationsQuery, ParentNotificationsResponseDto>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetParentNotificationsQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<ParentNotificationsResponseDto> Handle(GetParentNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetByUserId(
                request.UserId, request.Page, request.Limit, cancellationToken);

            var unreadCount = await _notificationRepository.GetUnreadCountAsync(request.UserId, cancellationToken);

            var items = notifications.Select(n => new ParentNotificationItemDto
            {
                id = n.PublicId.ToString(),
                type = MapNotificationType(n.NotificationType),
                title = MapNotificationTitle(n.NotificationType),
                message = n.Message,
                time_ago = GetTimeAgo(n.NotificationDate),
                is_read = n.IsRead
            }).ToList();

            return new ParentNotificationsResponseDto
            {
                unread_count = unreadCount,
                notifications = items
            };
        }

        private static string MapNotificationType(NotificationType type)
        {
            return type switch
            {
                NotificationType.Announcement => "announcement",
                NotificationType.NewLesson => "lesson",
                NotificationType.StreakReminder => "streak",
                NotificationType.HomeworkReminder => "homework",
                NotificationType.AiProgressReport => "report",
                NotificationType.Achievement => "achievement",
                NotificationType.NewMaterial => "material",
                _ => "system"
            };
        }

        private static string MapNotificationTitle(NotificationType type)
        {
            return type switch
            {
                NotificationType.Announcement => "Announcement",
                NotificationType.NewLesson => "New Lesson",
                NotificationType.StreakReminder => "Streak Reminder",
                NotificationType.HomeworkReminder => "Homework Reminder",
                NotificationType.AiProgressReport => "AI Progress Report",
                NotificationType.Achievement => "Achievement Unlocked",
                NotificationType.NewMaterial => "New Material",
                _ => "Notification"
            };
        }

        private static string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} min ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            return $"{(int)span.TotalDays}d ago";
        }
    }
}
