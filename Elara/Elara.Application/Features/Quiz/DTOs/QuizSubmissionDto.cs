

namespace Elara.Application.Features.Quiz.DTOs
{
    public class SubmitAnswerRequest
    {
        public int QuestionId { get; set; }
        public string QuestionType { get; set; } = string.Empty;
        public int? SelectedOptionId { get; set; }
        public string? AnswerContent { get; set; }
        public bool HintUsed { get; set; } = false;
    }

    public class SubmitAnswerResponse
    {
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public int? CorrectOptionId { get; set; }
        public int XpAwarded { get; set; }
    }

    public class QuizResultDto
    {
        public int SessionId { get; set; }
        public DateTime CompletedAt { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string LessonTitle { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public QuizStatsDto Results { get; set; } = new();
        public StudentProgressDto StudentProgress { get; set; } = new();
        public ElaraInsightDto ElaraInsight { get; set; } = new();
    }

    public class QuizStatsDto
    {
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int UnansweredCount { get; set; }
        public int TotalQuestions { get; set; }
        public double AccuracyPercentage { get; set; }
        public int XpEarned { get; set; }
        public XpBreakdownDto XpBreakdown { get; set; } = new();
    }

    public class XpBreakdownDto
    {
        public int BaseXP { get; set; }
        public int FinalXP { get; set; }
    }

    public class StudentProgressDto
    {
        public int TotalXP { get; set; }
        public int Level { get; set; }
        public int CurrentStreak { get; set; }
    }

    public class ElaraInsightDto
    {
        public string Message { get; set; } = string.Empty;
        public List<string> WeakTopics { get; set; } = new();
        public string Recommendation { get; set; } = string.Empty;
    }
}
