using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Submissions;


namespace Elara.Application.Common.Interfaces
{
    public interface IQuizService
    {
        Task<int> GenerateQuizAsync(int lessonId, int questionCount, string difficulty, List<string> questionTypes, CancellationToken cancellationToken = default);
        Task<int> CalculateTotalXpAsync(QuizSession session, CancellationToken cancellationToken = default);
        Task<(bool IsCorrect, int Score, string Feedback)> GradeEssayAnswerAsync(string questionText, string studentAnswer, CancellationToken cancellationToken = default);
        Task<ElaraInsightDto> GenerateQuizInsightAsync(QuizSession session, CancellationToken cancellationToken = default);
        Task UpdateStudentProgressAsync(Guid studentId, int xpEarned, CancellationToken cancellationToken = default);
    }
}
