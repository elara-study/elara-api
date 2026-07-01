namespace Elara.Application.Features.Users.Parents.Queries.GetChildHomeworkSubmission
{
    public class ChildHomeworkSubmissionDto
    {
        public string total_score { get; set; } = string.Empty;
        public List<SubmissionAnswerItemDto> answers { get; set; } = [];
    }

    public class SubmissionAnswerItemDto
    {
        public string problem_label { get; set; } = string.Empty;
        public bool is_correct { get; set; }
        public string question_text { get; set; } = string.Empty;
        public StudentAnswerDetailDto student_answer { get; set; } = new();
    }

    public class StudentAnswerDetailDto
    {
        public string type { get; set; } = "text";
        public string? url { get; set; }
        public string? text { get; set; }
    }
}
