using Elara.Application.Common;
using MediatR;

namespace Elara.Application.Features.Chat.Queries.GetConversations
{
    public class GetConversationsQuery : IRequest<List<ConversationSummaryDto>>
    {
        public PaginationParams Pagination { get; set; } = new();
    }
}
