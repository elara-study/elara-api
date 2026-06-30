using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.EditAnnouncement
{
    public class EditAnnouncementCommandHandler : IRequestHandler<EditAnnouncementCommand, AddAnnouncementResponse>
    {
        private readonly IClassRepository _classRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public EditAnnouncementCommandHandler(
            IClassRepository classRepository,
            IAnnouncementRepository announcementRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _classRepository = classRepository;
            _announcementRepository = announcementRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<AddAnnouncementResponse> Handle(EditAnnouncementCommand request, CancellationToken cancellationToken)
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

            if (request.Title != null)
                announcement.Title = request.Title;
            if (request.Content != null)
                announcement.Content = request.Content;

            await _announcementRepository.UpdateAsync(announcement, cancellationToken);

            return _mapper.Map<AddAnnouncementResponse>(announcement);
        }
    }
}
