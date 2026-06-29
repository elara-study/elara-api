using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteGroupStudent
{
    public class DeleteGroupStudentCommandHandler : IRequestHandler<DeleteGroupStudentCommand, Unit>
    {
        private readonly IClassRepository _classRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteGroupStudentCommandHandler(
            IClassRepository classRepository,
            ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteGroupStudentCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var internalClassId = await _classRepository.GetInternalIdByPublicIdAsync(request.ClassId, teacherId, cancellationToken);
            if (internalClassId == null)
            {
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found or you don't have access to it.");
            }

            var isJoined = await _classRepository.IsStudentJoinedClassAsync(request.StudentId, internalClassId.Value, cancellationToken);
            if (!isJoined)
            {
                throw new KeyNotFoundException($"Student with ID {request.StudentId} is not in this class.");
            }

            await _classRepository.RemoveStudentFromClassAsync(request.StudentId, internalClassId.Value, cancellationToken);

            return Unit.Value;
        }
    }
}
