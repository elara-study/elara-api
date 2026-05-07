using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.persistence.Administrative;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement
{
    public class AddAnnouncementCommandHandler : IRequestHandler<AddAnnouncementCommand, AddAnnouncementResponse>
    {
        private readonly IClassRepository _classRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationPreferenceRepository _notificationPreferenceRepository;
        private readonly INotificationService _notificationService;

        public AddAnnouncementCommandHandler(
            IClassRepository classRepository,
            IAnnouncementRepository announcementRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            INotificationRepository notificationRepository,
            INotificationPreferenceRepository notificationPreferenceRepository,
            INotificationService notificationService)
        {
            _classRepository = classRepository;
            _announcementRepository = announcementRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _notificationPreferenceRepository = notificationPreferenceRepository;
            _notificationService = notificationService;
        }

        public async Task<AddAnnouncementResponse> Handle(AddAnnouncementCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var classId = await _classRepository.GetInternalIdByPublicIdAsync(request.ClassId, teacherId, cancellationToken);

            if (classId == null)
            {
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found.");
            }

            var announcement = _mapper.Map<Announcement>(request);
            announcement.ClassId = classId.Value;

            await _announcementRepository.AddAsync(announcement, cancellationToken);

            var studentIds = await _classRepository.GetStudentIdsByClassIdAsync(classId.Value, cancellationToken);

            if (studentIds.Any())
            {
                var enabledStudentIds = await _notificationPreferenceRepository
                    .GetUsersWithPreferenceEnabledAsync(studentIds, NotificationType.Announcement, cancellationToken);

                if (enabledStudentIds.Any())
                {
                    var notifications = enabledStudentIds.Select(sid => new Notification
                    {
                        UserId = sid,
                        Message = announcement.Title,
                        NotificationType = NotificationType.Announcement,
                        NotificationDate = DateTime.UtcNow
                    }).ToList();

                    await _notificationRepository.AddRangeAsync(notifications, cancellationToken);
                }
            }

            _ = _notificationService.SendToTopicAsync(
                $"class_{request.ClassId}",
                "New Announcement",
                announcement.Title,
                new Dictionary<string, string>
                {
                    ["type"] = "announcement",
                    ["route"] = $"/classes/{request.ClassId}/announcements/{announcement.PublicId}"
                },
                cancellationToken);

            return _mapper.Map<AddAnnouncementResponse>(announcement);
        }
    }
}
