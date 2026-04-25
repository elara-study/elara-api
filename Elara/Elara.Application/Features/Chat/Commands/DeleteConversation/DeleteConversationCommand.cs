using MediatR;

namespace Elara.Application.Features.Chat.Commands.DeleteConversation
{
    public class DeleteConversationCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
