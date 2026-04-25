using MediatR;

namespace Elara.Application.Features.Chat.Commands.StartConversation
{
    public class StartConversationCommand : IRequest<StartConversationResponse>
    {
        public string Message { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
    }
}
