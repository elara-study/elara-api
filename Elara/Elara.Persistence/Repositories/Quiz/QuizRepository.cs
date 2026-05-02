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
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Topic)
                        .ThenInclude(t => t.Subject)
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Questions)
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
        }

        public async Task<List<Question>> GetQuestionsWithOptionsAsync(int assignmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Questions
                .Include(q => q.Options)
                .Include(q => q.Hints)
                .Where(q => q.AssignmentId == assignmentId)
                .OrderBy(q => q.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<QuizAnswer?> GetAnswerAsync(int sessionId, int questionId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizAnswers
                .FirstOrDefaultAsync(a => a.QuizSessionId == sessionId && a.QuestionId == questionId, cancellationToken);
        }

        public async Task<Assignment?> GetAssignmentWithDetailsAsync(int assignmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Assignments
                .Include(a => a.Topic)
                    .ThenInclude(t => t.Subject)
                .Include(a => a.Questions)
                .FirstOrDefaultAsync(a => a.Id == assignmentId, cancellationToken);
        }

        public async Task<QuizSession?> GetSessionWithDetailsAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizSessions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Lesson)
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Topic)
                        .ThenInclude(t => t.Subject)
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Questions)
                .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
        }

        public async Task<Question?> GetQuestionWithDetailsAsync(int questionId, CancellationToken cancellationToken = default)
        {
            return await _context.Questions
                .Include(q => q.Options)
                .Include(q => q.Hints)
                .FirstOrDefaultAsync(q => q.Id == questionId, cancellationToken);
        }

        public async Task<(List<QuizSession> sessions, int totalCount)> GetStudentQuizHistoryAsync(
            Guid studentId, int? lessonId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.QuizSessions
                .Include(s => s.Assignment)
                .Where(s => s.StudentId == studentId);

            if (lessonId.HasValue)
            {
                query = query.Where(s => s.Assignment.LessonId == lessonId.Value);
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
