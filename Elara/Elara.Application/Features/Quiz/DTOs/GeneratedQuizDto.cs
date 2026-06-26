namespace Elara.Application.Features.Quiz.DTOs
{
    public class GeneratedQuizDto
    {
        public int SessionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public List<QuizQuestionDto> Questions { get; set; } = new();
    }
}
