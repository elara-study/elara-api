using MediatR;

namespace Elara.Application.Features.Chat.Commands.SendMessage
{
    public class SendMessageCommand : IRequest<SendMessageResponse>
    {
        public Guid ConversationId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
