using Elara.Domain.Entities.Chat;

namespace Elara.Application.Contracts.Persistence.Chat
{
    public interface IChatRepository
    {
        Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Conversation>> GetByStudentIdAsync(Guid studentId, int page, int limit, CancellationToken cancellationToken = default);
        Task<Conversation> AddConversationAsync(Conversation conversation, CancellationToken cancellationToken = default);
        Task AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);
        Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ChatMessage>> GetLastNMessagesAsync(Guid conversationId, int n, CancellationToken cancellationToken = default);
        Task DeleteConversationAsync(Conversation conversation, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Conversation>> GetConversationsNeedingAnalysisAsync(int minMessages, CancellationToken ct = default);
        Task<IReadOnlyList<ChatMessage>> GetAllMessagesAsync(Guid conversationId, CancellationToken ct = default);
        Task UpsertReportAsync(ChatAnalysisReport report, CancellationToken ct = default);
        Task<ChatAnalysisReport?> GetReportByConversationIdAsync(Guid conversationId, CancellationToken ct = default);
        Task<IReadOnlyList<ChatAnalysisReport>> GetReportsByStudentIdsAsync(IEnumerable<Guid> studentIds, CancellationToken ct = default);
        Task<IReadOnlyList<ChatAnalysisReport>> GetReportsByStudentIdAsync(Guid studentId, CancellationToken ct = default);
        Task<IReadOnlyList<ChatAnalysisReport>> GetReportsByStudentIdAndSubjectAsync(Guid studentId, string subject, CancellationToken ct = default);
    }
}
