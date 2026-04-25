using MediatR;

namespace Elara.Application.Features.Chat.Queries.GetConversationHistory
{
    public class GetConversationHistoryQuery : IRequest<ConversationHistoryDto>
    {
        public Guid ConversationId { get; set; }
    }
}
