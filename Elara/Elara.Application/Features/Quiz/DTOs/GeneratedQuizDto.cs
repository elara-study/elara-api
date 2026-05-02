namespace Elara.Application.Features.Quiz.DTOs
{
    public class GeneratedQuizDto
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int MaxScore { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public int LessonId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
    }
}
