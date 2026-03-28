using System;

namespace Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement
{
    public class AddAnnouncementResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
