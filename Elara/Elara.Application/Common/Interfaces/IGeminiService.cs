using Elara.Application.Features.Chat;

namespace Elara.Application.Common.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GenerateResponseAsync(
            string userMessage,
            string ragContext,
            IEnumerable<ChatMessageDto> conversationHistory,
            CancellationToken cancellationToken = default);
    }
}
