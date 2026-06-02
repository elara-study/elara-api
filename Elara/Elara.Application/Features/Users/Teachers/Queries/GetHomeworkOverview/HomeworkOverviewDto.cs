namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview
{
    public class HomeworkOverviewDto
    {
        public int AssignmentId { get; set; }
        public string Topic { get; set; } = string.Empty;
        public HomeworkOverviewStats Overview { get; set; } = new();
        public List<HomeworkProblemDto> Problems { get; set; } = new();
    }

    public class HomeworkOverviewStats
    {
        public int TotalScore { get; set; }
    }

    public class HomeworkProblemDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = "text";
        public bool AllowImageUpload { get; set; } = true;
    }
}
