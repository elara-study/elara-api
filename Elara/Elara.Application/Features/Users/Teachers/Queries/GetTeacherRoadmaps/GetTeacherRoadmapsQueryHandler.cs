using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Users;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps
{
    public class GetTeacherRoadmapsQueryHandler : IRequestHandler<GetTeacherRoadmapsQuery, List<TeacherRoadmapListDto>>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetTeacherRoadmapsQueryHandler(
            ITeacherRepository teacherRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _teacherRepository = teacherRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<List<TeacherRoadmapListDto>> Handle(GetTeacherRoadmapsQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User must be authenticated");

            var roadmaps = await _teacherRepository.GetRoadmapsByTeacherAsync(teacherId, cancellationToken);

            return _mapper.Map<List<TeacherRoadmapListDto>>(roadmaps);
        }
    }
}
