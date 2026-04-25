using Elara.Domain.Enums;

namespace Elara.Application.Features.Chat
{
    public class ChatMessageDto
    {
        public MessageRole Role { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
