namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkSubmissions
{
    public class HomeworkSubmissionDto
    {
        public int SubmissionId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int? SubmittedAnswers { get; set; }
        public int? TotalProblems { get; set; }
        public double? Score { get; set; }
        public double? MaxScore { get; set; }
    }
}
