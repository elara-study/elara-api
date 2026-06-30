namespace Elara.Application.Features.Users.Teachers.Commands.GradeStudentSubmission
{
    public class GradeStudentSubmissionRequest
    {
        public double Score { get; set; }
        public string? Feedback { get; set; }
    }
}
