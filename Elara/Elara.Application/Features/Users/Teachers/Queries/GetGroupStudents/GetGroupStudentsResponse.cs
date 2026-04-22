using System;

namespace Elara.Application.Features.Users.Teachers.Queries.GetGroupStudents
{
    public class GetGroupStudentsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime EnrolledDate { get; set; }
    }
}
