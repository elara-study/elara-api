using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Educational;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Elara.Application.Contracts.Persistence.Quiz
{
    public interface IQuizRepository : IAsyncRepository<QuizSession, int>
    {
        Task<QuizSession?> GetSessionWithAnswersAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<List<Question>> GetQuestionsWithOptionsAsync(int assignmentId, CancellationToken cancellationToken = default);
        Task<QuizAnswer?> GetAnswerAsync(int sessionId, int questionId, CancellationToken cancellationToken = default);
        Task<(List<QuizSession> sessions, int totalCount)> GetStudentQuizHistoryAsync(Guid studentId, int? lessonId, int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
