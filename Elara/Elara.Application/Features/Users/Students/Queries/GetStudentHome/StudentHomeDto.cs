namespace Elara.Application.Features.Users.Students.Queries.GetStudentHome
{
    public class StudentHomeDto
    {
        public string StudentName { get; set; } = string.Empty;
        public HomeGamificationDto Gamification { get; set; } = new();
        public HomeRecentActivityDto? RecentActivity { get; set; }
        public HomeDailyGoalsDto DailyGoals { get; set; } = new();
        public List<HomeGroupDto> MyGroups { get; set; } = new();
    }

    public class HomeGamificationDto
    {
        public int Streak { get; set; }
        public int TotalXp { get; set; }
    }

    public class HomeRecentActivityDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int CurrentLessonNumber { get; set; }
        public int TotalLessons { get; set; }
        public int ProgressPercentage { get; set; }
        public string ActionUrl { get; set; } = string.Empty;
    }

    public class HomeDailyGoalsDto
    {
        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }
        public List<HomeGoalItemDto> Goals { get; set; } = new();
    }

    public class HomeGoalItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TargetValue { get; set; }
        public int CurrentValue { get; set; }
        public int XpReward { get; set; }
        public bool IsCompleted { get; set; }
        public string IconType { get; set; } = "flag";
    }

    public class HomeGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
