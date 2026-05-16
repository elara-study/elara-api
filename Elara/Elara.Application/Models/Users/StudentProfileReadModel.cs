using Elara.Domain.Enums;

namespace Elara.Application.Models.Users
{
    /// <summary>
    /// Read model for the student profile dashboard. <see cref="Parents"/> is projected from StudentParents.
    /// </summary>
    public class StudentProfileReadModel
    {
        public Guid StudentId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public GradeLevel GradeLevel { get; set; }
        public int TotalXP { get; set; }
        public int CurrentStreak { get; set; }
        public IReadOnlyList<ParentProfileReadModel> Parents { get; set; } = [];
        public IReadOnlyList<StudentAchievementReadModel> RecentAchievements { get; set; } = [];
    }

    public class ParentProfileReadModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }

    public class StudentAchievementReadModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
