using MediatR;
using System;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteGroupStudent
{
    public class DeleteGroupStudentCommand : IRequest<Unit>
    {
        public Guid ClassId { get; }
        public Guid StudentId { get; }

        public DeleteGroupStudentCommand(Guid classId, Guid studentId)
        {
            ClassId = classId;
            StudentId = studentId;
        }
    }
}
