using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Submissions;

namespace Elara.Application.Common.Interfaces
{
    public interface IQuizService
    {
        Task<GenerateQuizResult> GenerateQuizAsync(int moduleId, int questionCount, string difficulty, List<string> questionTypes, CancellationToken cancellationToken = default);
        Task<int> CalculateTotalXpAsync(QuizSession session, CancellationToken cancellationToken = default);
        Task<(bool IsCorrect, int Score, string Feedback)> GradeEssayAnswerAsync(string questionText, string studentAnswer, CancellationToken cancellationToken = default);
        Task<string> GenerateQuizInsightAsync(int correctCount, int totalCount, string quizTitle, CancellationToken cancellationToken = default);
        Task UpdateStudentProgressAsync(Guid studentId, int xpEarned, CancellationToken cancellationToken = default);
    }

    public class GenerateQuizResult
    {
        public string QuestionsJson { get; set; } = string.Empty;
        public string QuizTitle { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
    }
}
