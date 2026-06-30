namespace Elara.Application.Features.Users.Teachers.Queries.GetStudentDetail
{
    public class StudentDetailResponse
    {
        public StudentDetailUserDto User { get; set; } = new();
        public StudentDetailGamificationDto Gamification { get; set; } = new();
        public StudentDetailStatsDto Stats { get; set; } = new();
        public List<StudentDetailParentDto> Parents { get; set; } = [];
        public List<StudentDetailInsightDto> Insights { get; set; } = [];
    }

    public class StudentDetailUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string GradeLevel { get; set; } = string.Empty;
    }

    public class StudentDetailGamificationDto
    {
        public int CurrentLevel { get; set; }
        public int CurrentXp { get; set; }
        public int NextLevelXpThreshold { get; set; }
        public int XpToNextLevel { get; set; }
        public double ProgressPercentage { get; set; }
    }

    public class StudentDetailStatsDto
    {
        public int TotalXp { get; set; }
        public int LessonsCompleted { get; set; }
        public int TotalLessons { get; set; }
        public int StreakDays { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class StudentDetailParentDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }

    public class StudentDetailInsightDto
    {
        public Guid Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
