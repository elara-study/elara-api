using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Domain.Entities.Chat;
using Elara.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace Elara.Application.Features.Chat.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResponse>
    {
        private readonly IGeminiService _geminiService;
        private readonly IRagService _ragService;
        private readonly IChatRepository _chatRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ChatSettings _chatSettings;

        public SendMessageCommandHandler(IGeminiService geminiService, IRagService ragService, IChatRepository chatRepository, ICurrentUserService currentUserService, IOptions<ChatSettings> chatSettings)
        {
            _geminiService = geminiService;
            _ragService = ragService;
            _chatRepository = chatRepository;
            _currentUserService = currentUserService;
            _chatSettings = chatSettings.Value;
        }

        public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var studentId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Student must be authenticated.");

            var conversation = await _chatRepository.GetByIdWithMessagesAsync(request.ConversationId, cancellationToken)
                ?? throw new KeyNotFoundException($"Conversation '{request.ConversationId}' not found.");

            if (conversation.StudentId != studentId)
                throw new UnauthorizedAccessException("You do not have access to this conversation.");

            var history = await _chatRepository.GetLastNMessagesAsync(
                request.ConversationId, _chatSettings.ContextWindowSize, cancellationToken);

            var historyDtos = history.Select(m => new ChatMessageDto
            {
                Role = m.Role,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            });

            var ragContext = await _ragService.GetRelevantChunksAsync(
                request.Message, conversation.Subject, cancellationToken);

            var aiReply = await _geminiService.GenerateResponseAsync(
                request.Message, ragContext, historyDtos, cancellationToken);

            await _chatRepository.AddMessageAsync(new ChatMessage
            {
                ConversationId = request.ConversationId,
                Role = MessageRole.Student,
                Content = request.Message
            }, cancellationToken);

            await _chatRepository.AddMessageAsync(new ChatMessage
            {
                ConversationId = request.ConversationId,
                Role = MessageRole.AI,
                Content = aiReply
            }, cancellationToken);

            return new SendMessageResponse { AiReply = aiReply };
        }
    }
}
