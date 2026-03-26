using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses
{
    public class GetTeacherClassesQueryHandler : IRequestHandler<GetTeacherClassesQuery, List<GetTeacherClassesResponse>>
    {
        private readonly IClassRepository _classRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetTeacherClassesQueryHandler(IClassRepository classRepository, ICurrentUserService currentUserService, IMapper mapper)
        {
            _classRepository = classRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<List<GetTeacherClassesResponse>> Handle(GetTeacherClassesQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var classes = await _classRepository.GetClassesByTeacherIdAsync(teacherId, cancellationToken);

            return _mapper.Map<List<GetTeacherClassesResponse>>(classes);
        }
    }
}
