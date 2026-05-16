namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherProfile
{
    public class TeacherProfileDto
    {
        public TeacherProfileUserDto User { get; set; } = new();
        public TeacherProfileContactDto Contact { get; set; } = new();
        public TeacherProfileStatisticsDto Statistics { get; set; } = new();
    }

    public class TeacherProfileUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public IReadOnlyList<string> Subjects { get; set; } = [];
    }

    public class TeacherProfileContactDto
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    public class TeacherProfileStatisticsDto
    {
        public int TotalStudents { get; set; }
        public int ActiveGroups { get; set; }
        public int RoadmapsCreated { get; set; }
        public int LessonsPublished { get; set; }
    }
}
