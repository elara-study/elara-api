namespace Elara.Application.Features.Users.Teachers.Queries.GetStudentSubmission
{
    public class StudentSubmissionDetailDto
    {
        public int SubmissionId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public double? Score { get; set; }
        public double MaxScore { get; set; }
        public List<SubmissionAnswerDto> Answers { get; set; } = new();
    }

    public class SubmissionAnswerDto
    {
        public int ProblemId { get; set; }
        public string ProblemText { get; set; } = string.Empty;
        public string? StudentTextAnswer { get; set; }
        public string? StudentImageUrl { get; set; }
    }
}
