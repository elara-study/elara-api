namespace Elara.Application.Features.Quiz.DTOs
{
    public class QuizQuestionDto
    {
        public int QuestionNumber { get; set; }
        public string Text { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string? Hint { get; set; }
        public List<string> Options { get; set; } = new();
    }
}
