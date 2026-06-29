using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Educational;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetGroupRoadmap
{
    public class GetGroupRoadmapQueryHandler : IRequestHandler<GetGroupRoadmapQuery, TeacherRoadmapDetailDto>
    {
        private readonly IClassRepository _classRepository;
        private readonly IRoadmapRepository _roadmapRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetGroupRoadmapQueryHandler(
            IClassRepository classRepository,
            IRoadmapRepository roadmapRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _roadmapRepository = roadmapRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<TeacherRoadmapDetailDto> Handle(GetGroupRoadmapQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var classEntity = await _classRepository.GetClassWithSubjectByPublicIdAsync(request.ClassId, cancellationToken);
            if (classEntity == null)
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found.");

            if (classEntity.TeacherId != teacherId)
                throw new UnauthorizedAccessException("You do not have access to this class.");

            if (classEntity.RoadmapId == null)
                throw new KeyNotFoundException("No roadmap assigned to this class.");

            var roadmap = await _roadmapRepository.GetRoadmapWithDetailsAsync(classEntity.RoadmapId.Value, cancellationToken);
            if (roadmap == null)
                throw new KeyNotFoundException("Roadmap not found.");

            return _mapper.Map<TeacherRoadmapDetailDto>(roadmap);
        }
    }
}
