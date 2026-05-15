using System.Threading.Channels;
using Elara.Application.Common.Interfaces;
using Elara.Application.Features.ChatAnalysisReport;

namespace Elara.Infrastructure.Chat
{
    public class ChatAnalysisQueue : IChatAnalysisQueue
    {
        private readonly Channel<ChatAnalysisJob> _channel;

        public ChatAnalysisQueue(int capacity = 500)
        {
            _channel = Channel.CreateBounded<ChatAnalysisJob>(
                new BoundedChannelOptions(capacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = true
                });
        }

        public ValueTask EnqueueAsync(ChatAnalysisJob job, CancellationToken ct = default)
            => _channel.Writer.TryWrite(job)
                ? ValueTask.CompletedTask
                : _channel.Writer.WriteAsync(job, ct);

        public IAsyncEnumerable<ChatAnalysisJob> DequeueAllAsync(CancellationToken ct)
            => _channel.Reader.ReadAllAsync(ct);
    }
}
