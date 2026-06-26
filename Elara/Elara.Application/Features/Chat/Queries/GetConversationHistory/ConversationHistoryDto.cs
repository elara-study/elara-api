namespace Elara.Application.Features.Chat.Queries.GetConversationHistory
{
    public class ConversationHistoryDto
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ChatMessageDto> Messages { get; set; } = [];
    }
}
