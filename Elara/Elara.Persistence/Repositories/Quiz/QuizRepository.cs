using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories.Quiz
{
    public class QuizRepository : BaseRepository<QuizSession, int>, IQuizRepository
    {
        public QuizRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<QuizSession?> GetSessionWithAnswersAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizSessions
                .Include(s => s.Student)
                .Include(s => s.Module)
                    .ThenInclude(m => m.Subject)
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
        }

        public async Task<QuizAnswer?> GetAnswerAsync(int sessionId, int questionIndex, CancellationToken cancellationToken = default)
        {
            var session = await _context.QuizSessions
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

            if (session?.Answers == null || questionIndex >= session.Answers.Count)
                return null;

            return session.Answers.ElementAt(questionIndex);
        }

        public async Task<ProblemSet?> GetProblemSetWithDetailsAsync(int problemSetId, CancellationToken cancellationToken = default)
        {
            return await _context.ProblemSets
                .Include(a => a.Module)
                    .ThenInclude(t => t.Subject)
                .Include(a => a.Questions)
                .FirstOrDefaultAsync(a => a.Id == problemSetId, cancellationToken);
        }

        public async Task<QuizSession?> GetSessionWithDetailsAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizSessions
                .Include(s => s.Student)
                .Include(s => s.Module)
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
        }

        public async Task<(List<QuizSession> sessions, int totalCount)> GetStudentQuizHistoryAsync(
            Guid studentId, int? moduleId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.QuizSessions
                .Include(s => s.Module)
                .Where(s => s.StudentId == studentId);

            if (moduleId.HasValue)
            {
                query = query.Where(s => s.ModuleId == moduleId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var sessions = await query
                .OrderByDescending(s => s.StartedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (sessions, totalCount);
        }
    }
}
