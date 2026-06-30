using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Educational;

namespace Elara.Application.Contracts.Persistence.Quiz
{
    public interface IQuizRepository : IAsyncRepository<QuizSession, int>
    {
        Task<QuizSession?> GetSessionWithAnswersAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<QuizAnswer?> GetAnswerAsync(int sessionId, int questionId, CancellationToken cancellationToken = default);
        Task<Homework?> GetHomeworkWithDetailsAsync(int homeworkId, CancellationToken cancellationToken = default);
        Task<QuizSession?> GetSessionWithDetailsAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<(List<QuizSession> sessions, int totalCount)> GetStudentQuizHistoryAsync(Guid studentId, int? moduleId, int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
