using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Students.Commands.JoinGroup
{
    public class JoinGroupCommandHandler : IRequestHandler<JoinGroupCommand>
    {
        private readonly IClassRepository _classRepository;

        public JoinGroupCommandHandler(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task Handle(JoinGroupCommand request, CancellationToken cancellationToken)
        {
            var classEntity = await _classRepository.GetClassByJoinCodeAsync(request.JoinCode, cancellationToken);
            if (classEntity == null)
            {
                throw new KeyNotFoundException("Invalid join code.");
            }

            var isAlreadyJoined = await _classRepository.IsStudentJoinedClassAsync(request.StudentId, classEntity.Id, cancellationToken);
            if (isAlreadyJoined)
            {
                throw new InvalidOperationException("Student is already a member of this group.");
            }

            await _classRepository.JoinClassAsync(request.StudentId, classEntity.Id, cancellationToken);
        }
    }
}
