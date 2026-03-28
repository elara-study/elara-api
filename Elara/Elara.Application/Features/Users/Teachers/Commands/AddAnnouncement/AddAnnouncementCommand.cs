using MediatR;
using System;

namespace Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement
{
    public class AddAnnouncementCommand : IRequest<AddAnnouncementResponse>
    {
        public AddAnnouncementCommand(Guid classId, string title, string content)
        {
            ClassId = classId;
            Title = title;
            Content = content;
        }

        public Guid ClassId { get; }
        public string Title { get; }
        public string Content { get; }
    }
}
