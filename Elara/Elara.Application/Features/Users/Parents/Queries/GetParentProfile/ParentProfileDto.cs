namespace Elara.Application.Features.Users.Parents.Queries.GetParentProfile
{
    public class ParentProfileDto
    {
        public ParentUserDto user { get; set; } = new();
        public ParentStatsDto stats { get; set; } = new();
        public List<ParentChildDto> children { get; set; } = [];
    }

    public class ParentUserDto
    {
        public string name { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string avatar_url { get; set; } = string.Empty;
    }

    public class ParentStatsDto
    {
        public int avg_completion { get; set; }
        public int avg_attendance { get; set; }
    }

    public class ParentChildDto
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string avatar_url { get; set; } = string.Empty;
    }
}
