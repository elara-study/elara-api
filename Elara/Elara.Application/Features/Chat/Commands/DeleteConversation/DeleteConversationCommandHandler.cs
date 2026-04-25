using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using MediatR;

namespace Elara.Application.Features.Chat.Commands.DeleteConversation
{
    public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, Unit>
    {
        private readonly IChatRepository _chatRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteConversationCommandHandler(IChatRepository chatRepository, ICurrentUserService currentUserService)
        {
            _chatRepository = chatRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
        {
            var studentId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var conversation = await _chatRepository.GetByIdAsync(request.Id, cancellationToken);

            if (conversation == null)
            {
                throw new KeyNotFoundException($"Conversation with Id {request.Id} not found.");
            }

            if (conversation.StudentId != studentId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this conversation.");
            }

            await _chatRepository.DeleteConversationAsync(conversation, cancellationToken);

            return Unit.Value;
        }
    }
}
