using MediatR;
using System;

namespace Elara.Application.Features.Users.Teachers.Commands.AddStudentByUsername
{
    public class AddStudentByUsernameCommand : IRequest<bool>
    {
        public Guid ClassId { get; set; }
        public string Username { get; set; } = string.Empty;

        public AddStudentByUsernameCommand(Guid classId, string username)
        {
            ClassId = classId;
            Username = username;
        }
    }
}
