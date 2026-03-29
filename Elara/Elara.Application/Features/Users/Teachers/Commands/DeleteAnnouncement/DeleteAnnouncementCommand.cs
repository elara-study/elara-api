using MediatR;
using System;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteAnnouncement
{
    public class DeleteAnnouncementCommand : IRequest<Unit>
    {
        public DeleteAnnouncementCommand(Guid classId, Guid announcementId)
        {
            ClassId = classId;
            AnnouncementId = announcementId;
        }

        public Guid ClassId { get; }
        public Guid AnnouncementId { get; }
    }
}
