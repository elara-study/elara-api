namespace Elara.Application.Features.Users.Parents.Queries.GetParentDashboard
{
    public class ParentDashboardDto
    {
        public UserDto user { get; set; } = new();
        public List<ChildProgressDto> children { get; set; } = new();
        public OverallStatsDto overall_stats { get; set; } = new();
        public List<RecentActivityDto> recent_activity { get; set; } = new();
    }

    public class UserDto
    {
        public string name { get; set; } = string.Empty;
    }

    public class ChildProgressDto
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string avatar_url { get; set; } = string.Empty;
        public ProgressDto progress { get; set; } = new();
    }

    public class ProgressDto
    {
        public int current_lesson { get; set; }
        public int total_lessons { get; set; }
        public int completion_percentage { get; set; }
    }

    public class OverallStatsDto
    {
        public int avg_completion { get; set; }
        public int avg_attendance { get; set; }
    }

    public class RecentActivityDto
    {
        public string id { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string time_ago { get; set; } = string.Empty;
    }
}