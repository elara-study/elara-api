using Elara.Application.Common.Interfaces;
using MediatR;

namespace Elara.Application.Features.Chat.Queries.GetRelevantChunks
{
    public class GetRelevantChunksQueryHandler : IRequestHandler<GetRelevantChunksQuery, string>
    {
        private readonly IRagService _ragService;

        public GetRelevantChunksQueryHandler(IRagService ragService)
        {
            _ragService = ragService;
        }

        public async Task<string> Handle(GetRelevantChunksQuery request, CancellationToken cancellationToken)
        {
            return await _ragService.GetRelevantChunksAsync(request.Query, request.Subject, cancellationToken);
        }
    }
}
