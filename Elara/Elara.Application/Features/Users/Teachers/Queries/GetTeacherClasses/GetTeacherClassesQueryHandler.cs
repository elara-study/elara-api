using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses
{
    public class GetTeacherClassesQueryHandler : IRequestHandler<GetTeacherClassesQuery, List<GetTeacherClassesResponse>>
    {
        private readonly IClassRepository _classRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetTeacherClassesQueryHandler(IClassRepository classRepository, ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<GetTeacherClassesResponse>> Handle(GetTeacherClassesQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var classes = await _classRepository.GetClassesByTeacherIdAsync(teacherId, cancellationToken);

            return classes;
        }
    }
}
