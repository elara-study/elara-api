namespace Elara.Application.Features.Quiz.DTOs
{
    public class QuizSessionDto
    {
        public int SessionId { get; set; }
        public int AssignmentId { get; set; }
        public DateTime StartedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string LessonTitle { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
    }
}
