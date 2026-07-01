namespace Elara.Application.Features.Users.Parents.Queries.GetParentNotifications
{
    public class ParentNotificationsResponseDto
    {
        public int unread_count { get; set; }
        public List<ParentNotificationItemDto> notifications { get; set; } = [];
    }

    public class ParentNotificationItemDto
    {
        public string id { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string time_ago { get; set; } = string.Empty;
        public bool is_read { get; set; }
    }
}
