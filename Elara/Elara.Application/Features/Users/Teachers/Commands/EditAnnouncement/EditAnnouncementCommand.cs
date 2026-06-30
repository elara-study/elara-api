using Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.EditAnnouncement
{
    public class EditAnnouncementCommand : IRequest<AddAnnouncementResponse>
    {
        public Guid ClassId { get; }
        public Guid AnnouncementId { get; }
        public string? Title { get; }
        public string? Content { get; }

        public EditAnnouncementCommand(Guid classId, Guid announcementId, string? title, string? content)
        {
            ClassId = classId;
            AnnouncementId = announcementId;
            Title = title;
            Content = content;
        }
    }
}
