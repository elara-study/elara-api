using MediatR;
using System;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteClass
{
    public class DeleteClassCommand : IRequest<Unit>
    {
        public Guid ClassId { get; }

        public DeleteClassCommand(Guid classId)
        {
            ClassId = classId;
        }
    }
}
