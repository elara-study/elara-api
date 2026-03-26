using MediatR;

namespace Elara.Application.Features.Users.Students.Commands.JoinGroup
{
    public class JoinGroupCommand : IRequest
    {
        public JoinGroupCommand(Guid studentId, string joinCode)
        {
            StudentId = studentId;
            JoinCode = joinCode;
        }

        public Guid StudentId { get; }
        public string JoinCode { get; }
    }
}
