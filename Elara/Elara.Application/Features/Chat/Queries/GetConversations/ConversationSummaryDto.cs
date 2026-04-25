namespace Elara.Application.Features.Chat.Queries.GetConversations
{
    public class ConversationSummaryDto
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? LastMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
