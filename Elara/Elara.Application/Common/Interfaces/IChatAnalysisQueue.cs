using Elara.Application.Features.ChatAnalysisReport;

namespace Elara.Application.Common.Interfaces
{
    public interface IChatAnalysisQueue
    {
        ValueTask EnqueueAsync(ChatAnalysisJob job, CancellationToken ct = default);
        IAsyncEnumerable<ChatAnalysisJob> DequeueAllAsync(CancellationToken ct);
    }
}
