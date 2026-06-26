namespace Elara.Application.Features.Quiz.DTOs
{
    public class SubmitAnswerRequest
    {
        public int QuestionNumber { get; set; }
        public string QuestionType { get; set; } = string.Empty;
        public string? SelectedOptionText { get; set; }
        public string? AnswerContent { get; set; }
        public bool HintUsed { get; set; } = false;
    }

    public class SubmitAnswerResponse
    {
        public int QuestionNumber { get; set; }
        public bool IsCorrect { get; set; }
        public string? CorrectAnswerText { get; set; }
        public int XpAwarded { get; set; }
    }

    public class QuizResultDto
    {
        public int SessionId { get; set; }
        public DateTime CompletedAt { get; set; }
        public string QuizTitle { get; set; } = string.Empty;

        public QuizStatsDto Results { get; set; } = new();
        public StudentProgressDto StudentProgress { get; set; } = new();
        public string? ElaraInsight { get; set; }
    }

    public class QuizStatsDto
    {
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int UnansweredCount { get; set; }
        public int TotalQuestions { get; set; }
        public double AccuracyPercentage { get; set; }
        public int XpEarned { get; set; }
    }

    public class StudentProgressDto
    {
        public int TotalXP { get; set; }
        public int Level { get; set; }
        public int CurrentStreak { get; set; }
    }


}
