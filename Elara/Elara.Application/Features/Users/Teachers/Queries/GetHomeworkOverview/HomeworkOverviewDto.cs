namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview
{
    public class HomeworkOverviewDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public int TotalScoreXp { get; set; }
        public List<HomeworkProblemDto> Problems { get; set; } = new();
    }

    public class HomeworkProblemDto
    {
        public int ProblemId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
