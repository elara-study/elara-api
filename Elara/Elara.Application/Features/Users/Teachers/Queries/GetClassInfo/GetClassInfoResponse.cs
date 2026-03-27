using System;

namespace Elara.Application.Features.Users.Teachers.Queries.GetClassInfo
{
    public class GetClassInfoResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string JoinCode { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
    }
}