namespace Elara.Application.Features.Users.Parents.Queries.GetHomeworkProblems
{
    public class ParentHomeworkProblemDto
    {
        public string problem_id { get; set; } = string.Empty;
        public string label { get; set; } = string.Empty;
        public string question_text { get; set; } = string.Empty;
    }
}
