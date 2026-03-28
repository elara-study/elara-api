using System;

namespace Elara.Application.Features.Users.Teachers.Queries.GetAnnouncements
{
    public class GetAnnouncementsResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
