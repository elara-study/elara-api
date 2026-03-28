using System;

namespace Elara.Application.Features.Users.Teachers.Commands.CreateClass
{
    public class CreateClassResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string JoinCode { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public Guid TeacherId { get; set; }
    }
}
