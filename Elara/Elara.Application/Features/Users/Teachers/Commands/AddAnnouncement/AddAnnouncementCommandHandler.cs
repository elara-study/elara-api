using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Domain.Entities.Administrative;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement
{
    public class AddAnnouncementCommandHandler : IRequestHandler<AddAnnouncementCommand, AddAnnouncementResponse>
    {
        private readonly IClassRepository _classRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AddAnnouncementCommandHandler(
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

            return _mapper.Map<AddAnnouncementResponse>(announcement);
        }
    }
}
