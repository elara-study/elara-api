using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Educational;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps
{
    public class GetTeacherRoadmapDetailQueryHandler : IRequestHandler<GetTeacherRoadmapDetailQuery, TeacherRoadmapDetailDto>
    {
        private readonly IRoadmapRepository _roadmapRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetTeacherRoadmapDetailQueryHandler(
            IRoadmapRepository roadmapRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _roadmapRepository = roadmapRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<TeacherRoadmapDetailDto> Handle(GetTeacherRoadmapDetailQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User must be authenticated");

            var roadmap = await _roadmapRepository.GetByPublicIdAsync(request.RoadmapId, cancellationToken);

            if (roadmap == null || roadmap.TeacherId != teacherId)
                throw new KeyNotFoundException("Roadmap not found.");

            return _mapper.Map<TeacherRoadmapDetailDto>(roadmap);
        }
    }
}
