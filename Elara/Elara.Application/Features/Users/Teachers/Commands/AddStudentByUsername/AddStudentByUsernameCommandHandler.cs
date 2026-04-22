using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Users;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddStudentByUsername
{
    public class AddStudentByUsernameCommandHandler : IRequestHandler<AddStudentByUsernameCommand, bool>
    {
        private readonly IClassRepository _classRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;

        public AddStudentByUsernameCommandHandler(IClassRepository classRepository, IStudentRepository studentRepository, ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(AddStudentByUsernameCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var internalClassId = await _classRepository.GetInternalIdByPublicIdAsync(request.ClassId, teacherId, cancellationToken);
            if (internalClassId == null)
            {
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found or you don't have access to it.");
            }

            var studentId = await _studentRepository.GetStudentIdByUsernameAsync(request.Username, cancellationToken);
            if (studentId == null)
            {
                throw new KeyNotFoundException($"Student with username '{request.Username}' not found.");
            }

            var isAlreadyJoined = await _classRepository.IsStudentJoinedClassAsync(studentId.Value, internalClassId.Value, cancellationToken);
            if (isAlreadyJoined)
            {
                throw new InvalidOperationException($"Student with username '{request.Username}' is already in this class.");
            }

            await _classRepository.JoinClassAsync(studentId.Value, internalClassId.Value, cancellationToken);

            return true;
        }
    }
}
