using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Application.Features.ChatAnalysisReport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elara.Infrastructure.Chat
{
    public class ChatAnalysisScheduler : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IChatAnalysisQueue _queue;
        private readonly ILogger<ChatAnalysisScheduler> _logger;
        private readonly ElaraReportSettings _settings;

        public ChatAnalysisScheduler(
            IServiceScopeFactory scopeFactory,
            IChatAnalysisQueue queue,
            ILogger<ChatAnalysisScheduler> logger,
            IOptions<ElaraReportSettings> settings)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "ChatAnalysisScheduler started. Running every {Interval} minutes.",
                _settings.RunIntervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DiscoverAndEnqueueAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduler discovery failed.");
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(_settings.RunIntervalMinutes),
                    stoppingToken);
            }
        }

        private async Task DiscoverAndEnqueueAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var chatRepo = scope.ServiceProvider.GetRequiredService<IChatRepository>();

            var conversations = await chatRepo
                .GetConversationsNeedingAnalysisAsync(_settings.MinMessageThreshold, ct);

            _logger.LogInformation(
                "Scheduler: found {Count} conversations to analyze.",
                conversations.Count);

            foreach (var conv in conversations)
            {
                await _queue.EnqueueAsync(
                    new ChatAnalysisJob(conv.Id, conv.StudentId, conv.Subject), ct);
            }
        }
    }
}
