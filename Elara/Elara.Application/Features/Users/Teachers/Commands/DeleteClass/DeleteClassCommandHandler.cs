using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteClass
{
    public class DeleteClassCommandHandler : IRequestHandler<DeleteClassCommand, Unit>
    {
        private readonly IClassRepository _classRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteClassCommandHandler(
            IClassRepository classRepository,
            ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var internalClassId = await _classRepository.GetInternalIdByPublicIdAsync(request.ClassId, teacherId, cancellationToken);
            if (internalClassId == null)
            {
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found or you don't have access to it.");
            }

            var classEntity = await _classRepository.GetByIdAsync(internalClassId.Value, cancellationToken);
            if (classEntity == null)
            {
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found.");
            }

            classEntity.IsDeleted = true;
            classEntity.DeletedAt = DateTime.UtcNow;
            await _classRepository.UpdateAsync(classEntity, cancellationToken);

            return Unit.Value;
        }
    }
}
