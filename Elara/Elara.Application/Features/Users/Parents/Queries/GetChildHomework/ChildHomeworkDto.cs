namespace Elara.Application.Features.Users.Parents.Queries.GetChildHomework
{
    public class ChildHomeworkDto
    {
        public HomeworkSummaryDto summary { get; set; } = new();
        public List<HomeworkListItemDto> homework_list { get; set; } = [];
    }

    public class HomeworkSummaryDto
    {
        public int total { get; set; }
        public int submitted { get; set; }
        public int graded { get; set; }
    }

    public class HomeworkListItemDto
    {
        public string id { get; set; } = string.Empty;
        public string module { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;
        public string class_label { get; set; } = string.Empty;
        public string status { get; set; } = "pending";
        public string? score { get; set; }
    }
}
