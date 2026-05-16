namespace Elara.Application.Models.Users
{
    public class TeacherProfileReadModel
    {
        public Guid TeacherId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public IReadOnlyList<string> Subjects { get; set; } = [];
        public int TotalStudents { get; set; }
        public int ActiveGroups { get; set; }
        public int RoadmapsCreated { get; set; }
        public int LessonsPublished { get; set; }
    }
}
