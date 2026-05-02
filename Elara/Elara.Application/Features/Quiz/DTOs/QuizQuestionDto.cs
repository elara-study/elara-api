namespace Elara.Application.Features.Quiz.DTOs
{
    public class QuizQuestionsListDto
    {
        public int AssignmentId { get; set; }
        public List<QuizQuestionDto> Questions { get; set; } = new();
    }

    public class QuizQuestionDto
    {
        public int QuestionId { get; set; }
        public int QuestionNumber { get; set; }
        public string Text { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public double Marks { get; set; }
        public bool HasHint { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = new();
    }

    public class QuestionOptionDto
    {
        public int OptionId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
