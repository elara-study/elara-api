using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteAnnouncement
{
    public class DeleteAnnouncementCommandHandler : IRequestHandler<DeleteAnnouncementCommand, Unit>
    {
        private readonly IClassRepository _classRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteAnnouncementCommandHandler(
            IClassRepository classRepository,
            IAnnouncementRepository announcementRepository,
            ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _announcementRepository = announcementRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteAnnouncementCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var classId = await _classRepository.GetInternalIdByPublicIdAsync(request.ClassId, teacherId, cancellationToken);

            if (classId == null)
            {
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found.");
            }

            var announcement = await _announcementRepository.GetByPublicIdAndClassIdAsync(
                request.AnnouncementId,
                classId.Value,
                cancellationToken);

            if (announcement == null)
            {
                throw new KeyNotFoundException($"Announcement with ID {request.AnnouncementId} not found.");
            }

            announcement.IsDeleted = true;
            announcement.DeletedAt = DateTime.UtcNow;

            await _announcementRepository.UpdateAsync(announcement, cancellationToken);

            return Unit.Value;
        }
    }
}
