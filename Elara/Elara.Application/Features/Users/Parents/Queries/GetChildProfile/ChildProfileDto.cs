namespace Elara.Application.Features.Users.Parents.Queries.GetChildProfile
{
    public class ChildProfileDto
    {
        public ChildInfoDto child_info { get; set; } = new();
        public LatestInsightDto? latest_insight { get; set; }
        public LatestHomeworkDto? latest_homework { get; set; }
    }

    public class ChildInfoDto
    {
        public string name { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string grade { get; set; } = string.Empty;
        public int level { get; set; }
        public ChildXpDto xp { get; set; } = new();
        public ChildProfileStatsDto stats { get; set; } = new();
    }

    public class ChildXpDto
    {
        public int current { get; set; }
        public int target_xp { get; set; }
        public int xp_needed { get; set; }
    }

    public class ChildProfileStatsDto
    {
        public int total_xp { get; set; }
        public string lessons { get; set; } = "0/0";
        public int day_streak { get; set; }
        public int attendance_percentage { get; set; }
    }

    public class LatestInsightDto
    {
        public string time_ago { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }

    public class LatestHomeworkDto
    {
        public string module_name { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;
        public string @class { get; set; } = string.Empty;
    }
}
