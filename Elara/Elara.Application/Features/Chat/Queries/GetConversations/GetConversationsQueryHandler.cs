using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using MediatR;

namespace Elara.Application.Features.Chat.Queries.GetConversations
{
    public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<ConversationSummaryDto>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetConversationsQueryHandler(IChatRepository chatRepository, ICurrentUserService currentUserService)
        {
            _chatRepository = chatRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<ConversationSummaryDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
        {
            var studentId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Student must be authenticated.");

            var conversations = await _chatRepository.GetByStudentIdAsync(
                studentId, request.Pagination.Page, request.Pagination.Limit, cancellationToken);

            return conversations.Select(c => new ConversationSummaryDto
            {
                Id = c.Id,
                Subject = c.Subject,
                CreatedAt = c.CreatedAt,
                LastMessage = c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault()?.Content
            }).ToList();
        }
    }
}
