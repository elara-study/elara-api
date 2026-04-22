using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetGroupStudents
{
    public class GetGroupStudentsQueryHandler : IRequestHandler<GetGroupStudentsQuery, List<GetGroupStudentsResponse>>
    {
        private readonly IClassRepository _classRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetGroupStudentsQueryHandler(IClassRepository classRepository, ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<GetGroupStudentsResponse>> Handle(GetGroupStudentsQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            // Verify the class exists and belongs to the teacher
            var existsAndOwned = await _classRepository.ExistsAndOwnedByTeacherAsync(request.ClassId, teacherId, cancellationToken);
            if (!existsAndOwned)
            {
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found or you don't have access to it.");
            }

            var students = await _classRepository.GetStudentsInClassAsync(request.ClassId, teacherId, cancellationToken);
            return students;
        }
    }
}
