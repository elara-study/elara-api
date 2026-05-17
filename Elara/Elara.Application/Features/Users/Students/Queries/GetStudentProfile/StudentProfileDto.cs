namespace Elara.Application.Features.Users.Students.Queries.GetStudentProfile
{
    public class StudentProfileDto
    {
        public StudentProfileUserDto User { get; set; } = new();
        public StudentProfileGamificationDto Gamification { get; set; } = new();
        public IReadOnlyList<StudentProfileParentDto> Parents { get; set; } = [];
        public IReadOnlyList<StudentProfileAchievementDto> RecentAchievements { get; set; } = [];
    }

    public class StudentProfileUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string GradeLevel { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }

    public class StudentProfileGamificationDto
    {
        public StudentProfileLevelDto Level { get; set; } = new();
        public StudentProfileXpDto Xp { get; set; } = new();
        public StudentProfileStatisticsDto Statistics { get; set; } = new();
    }

    public class StudentProfileLevelDto
    {
        public int Current { get; set; }
        public int Next { get; set; }
    }

    public class StudentProfileXpDto
    {
        public int Current { get; set; }
        public int Target { get; set; }
        public int RemainingToNextLevel { get; set; }
    }

    public class StudentProfileStatisticsDto
    {
        public int DayStreak { get; set; }
        public int TotalXp { get; set; }
        public int QuizzesCompleted { get; set; }
    }

    public class StudentProfileParentDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }

    public class StudentProfileAchievementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
