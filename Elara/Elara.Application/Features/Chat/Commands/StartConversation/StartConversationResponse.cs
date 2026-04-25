namespace Elara.Application.Features.Chat.Commands.StartConversation
{
    public class StartConversationResponse
    {
        public Guid ConversationId { get; set; }
        public string AiReply { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
    }
}
