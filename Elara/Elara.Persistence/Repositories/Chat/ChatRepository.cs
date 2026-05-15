using Elara.Application.Contracts.Persistence.Chat;
using Elara.Domain.Entities.Chat;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories.Chat
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Conversations.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Conversations
                .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Conversation>> GetByStudentIdAsync(Guid studentId, int page, int limit, CancellationToken cancellationToken = default)
        {
            return await _context.Conversations
                .Where(c => c.StudentId == studentId)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task<Conversation> AddConversationAsync(Conversation conversation, CancellationToken cancellationToken = default)
        {
            await _context.Conversations.AddAsync(conversation, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return conversation;
        }

        public async Task AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            await _context.ChatMessages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ChatMessage>> GetLastNMessagesAsync(Guid conversationId, int n, CancellationToken cancellationToken = default)
        {
            return await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(n)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteConversationAsync(Conversation conversation, CancellationToken cancellationToken = default)
        {
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Conversation>> GetConversationsNeedingAnalysisAsync(
            int minMessages, CancellationToken ct = default)
        {
            return await _context.Conversations
                .Where(c => c.Messages.Count >= minMessages)
                .Where(c => !_context.ChatAnalysisReports.Any(r =>
                    r.ConversationId == c.Id &&
                    r.AnalyzedMessageCount >= c.Messages.Count))
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<ChatMessage>> GetAllMessagesAsync(
            Guid conversationId, CancellationToken ct = default)
        {
            return await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task UpsertReportAsync(ChatAnalysisReport report, CancellationToken ct = default)
        {
            var existing = await _context.ChatAnalysisReports
                .FirstOrDefaultAsync(r => r.ConversationId == report.ConversationId, ct);

            if (existing != null)
            {
                existing.ReportText = report.ReportText;
                existing.AnalyzedMessageCount = report.AnalyzedMessageCount;
                existing.Subject = report.Subject;
            }
            else
            {
                await _context.ChatAnalysisReports.AddAsync(report, ct);
            }

            await _context.SaveChangesAsync(ct);
        }

        public async Task<ChatAnalysisReport?> GetReportByConversationIdAsync(
            Guid conversationId, CancellationToken ct = default)
        {
            return await _context.ChatAnalysisReports
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ConversationId == conversationId, ct);
        }

        public async Task<IReadOnlyList<ChatAnalysisReport>> GetReportsByStudentIdsAsync(
            IEnumerable<Guid> studentIds, CancellationToken ct = default)
        {
            return await _context.ChatAnalysisReports
                .AsNoTracking()
                .Where(r => studentIds.Contains(r.StudentId))
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<ChatAnalysisReport>> GetReportsByStudentIdAsync(
            Guid studentId, CancellationToken ct = default)
        {
            return await _context.ChatAnalysisReports
                .AsNoTracking()
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<ChatAnalysisReport>> GetReportsByStudentIdAndSubjectAsync(
            Guid studentId, string subject, CancellationToken ct = default)
        {
            return await _context.ChatAnalysisReports
                .AsNoTracking()
                .Where(r => r.StudentId == studentId && r.Subject == subject)
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
