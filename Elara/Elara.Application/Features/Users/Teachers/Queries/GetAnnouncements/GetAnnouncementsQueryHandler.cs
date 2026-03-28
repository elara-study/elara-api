using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elara.Application.Features.Users.Teachers.Queries.GetAnnouncements
{
    public class GetAnnouncementsQueryHandler : IRequestHandler<GetAnnouncementsQuery, List<GetAnnouncementsResponse>>
    {
        private readonly IClassRepository _classRepository;
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAnnouncementsQueryHandler(
            IClassRepository classRepository,
            IAnnouncementRepository announcementRepository,
            ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _announcementRepository = announcementRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<GetAnnouncementsResponse>> Handle(GetAnnouncementsQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var isAuthorized = await _classRepository.ExistsAndOwnedByTeacherAsync(request.ClassId, teacherId, cancellationToken);
            
            if (!isAuthorized)
            {
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found.");
            }

            return await _announcementRepository.GetByClassPublicIdProjectedAsync(request.ClassId, cancellationToken);
        }
    }
}
