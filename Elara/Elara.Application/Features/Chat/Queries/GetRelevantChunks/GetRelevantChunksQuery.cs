using MediatR;

namespace Elara.Application.Features.Chat.Queries.GetRelevantChunks
{
    public class GetRelevantChunksQuery : IRequest<string>
    {
        public string Query { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
    }
}
