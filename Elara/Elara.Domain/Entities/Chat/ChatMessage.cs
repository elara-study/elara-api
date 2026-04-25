using Elara.Domain.Enums;


namespace Elara.Domain.Entities.Chat
{
    public class ChatMessage : BaseEntity<int>
    {
        public Guid ConversationId { get; set; }
        public MessageRole Role { get; set; }
        public string Content { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Conversation? Conversation { get; set; }
    }
}
