using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using MediatR;

namespace Elara.Application.Features.Chat.Queries.GetConversationHistory
{
    public class GetConversationHistoryQueryHandler : IRequestHandler<GetConversationHistoryQuery, ConversationHistoryDto>
    {
        private readonly IChatRepository _chatRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetConversationHistoryQueryHandler(IChatRepository chatRepository, ICurrentUserService currentUserService)
        {
            _chatRepository = chatRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ConversationHistoryDto> Handle(GetConversationHistoryQuery request, CancellationToken cancellationToken)
        {
            var studentId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Student must be authenticated.");

            var conversation = await _chatRepository.GetByIdWithMessagesAsync(request.ConversationId, cancellationToken)
                ?? throw new KeyNotFoundException($"Conversation '{request.ConversationId}' not found.");

            if (conversation.StudentId != studentId)
                throw new UnauthorizedAccessException("You do not have access to this conversation.");

            return new ConversationHistoryDto
            {
                Id = conversation.Id,
                Subject = conversation.Subject,
                Title = conversation.Title,
                CreatedAt = conversation.CreatedAt,
                Messages = conversation.Messages
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new ChatMessageDto
                    {
                        Role = m.Role,
                        Content = m.Content,
                        CreatedAt = m.CreatedAt
                    }).ToList()
            };
        }
    }
}
