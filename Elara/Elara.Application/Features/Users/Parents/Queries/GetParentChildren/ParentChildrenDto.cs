
namespace Elara.Application.Features.Users.Parents.Queries.GetParentChildren
{
    public class ParentChildrenDto
    {
        public List<PendingRequestDto> pending_requests { get; set; } = new();
        public List<MyChildDto> my_children { get; set; } = new();
    }

    public class PendingRequestDto
    {
        public string request_id { get; set; } = string.Empty;
        public string child_id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string requested_at { get; set; } = string.Empty;
    }

    public class MyChildDto
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int grade { get; set; }
        public int level { get; set; }
        public ChildStatsDto stats { get; set; } = new();
        public List<ChildSubjectProgressDto> subject_progress { get; set; } = new();
    }

    public class ChildStatsDto
    {
        public int day_streak { get; set; }
        public int total_xp { get; set; }
        public int lessons_completed { get; set; }
    }

    public class ChildSubjectProgressDto
    {
        public string subject { get; set; } = string.Empty;
        public int progress_percentage { get; set; }
    }
}
