using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Domain.Entities.Chat;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Chat.Commands.StartConversation
{
    public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand, StartConversationResponse>
    {
        private readonly IGeminiService _geminiService;
        private readonly IRagService _ragService;
        private readonly IChatRepository _chatRepository;
        private readonly ICurrentUserService _currentUserService;

        public StartConversationCommandHandler(IGeminiService geminiService, IRagService ragService, IChatRepository chatRepository, ICurrentUserService currentUserService)
        {
            _geminiService = geminiService;
            _ragService = ragService;
            _chatRepository = chatRepository;
            _currentUserService = currentUserService;
        }

        public async Task<StartConversationResponse> Handle(StartConversationCommand request, CancellationToken cancellationToken)
        {
            var studentId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Student must be authenticated.");

            var ragContext = await _ragService.GetRelevantChunksAsync(
                request.Message, request.Subject, cancellationToken);

            var aiReply = await _geminiService.GenerateResponseAsync(
                request.Message, ragContext, [], cancellationToken);

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                Subject = request.Subject,
                Messages = new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        Role = MessageRole.Student,
                        Content = request.Message
                    },
                    new ChatMessage
                    {
                        Role = MessageRole.AI,
                        Content = aiReply
                    }
                }
            };

            await _chatRepository.AddConversationAsync(conversation, cancellationToken);

            return new StartConversationResponse
            {
                ConversationId = conversation.Id,
                AiReply = aiReply,
                Subject = conversation.Subject
            };
        }
    }
}
