using System.Text;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.persistence.Administrative;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Chat;
using Elara.Domain.Enums;
using Elara.Application.Features.ChatAnalysisReport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Elara.Infrastructure.Chat
{
    public class ChatAnalysisWorker : BackgroundService
    {
        private readonly IChatAnalysisQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ChatAnalysisWorker> _logger;

        public ChatAnalysisWorker(
            IChatAnalysisQueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<ChatAnalysisWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ChatAnalysisWorker started, waiting for jobs...");

            await foreach (var job in _queue.DequeueAllAsync(stoppingToken))
            {
                try
                {
                    await ProcessJobAsync(job, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to process analysis for conversation {Id}",
                        job.ConversationId);
                }
            }
        }

        private async Task ProcessJobAsync(ChatAnalysisJob job, CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var chatRepo = scope.ServiceProvider.GetRequiredService<IChatRepository>();
            var reportService = scope.ServiceProvider.GetRequiredService<IElaraReportService>();
            var studentRepo = scope.ServiceProvider.GetRequiredService<IStudentRepository>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var deviceTokenRepo = scope.ServiceProvider.GetRequiredService<IDeviceTokenRepository>();
            var NotificationPreferenceService = scope.ServiceProvider.GetRequiredService<INotificationPreferenceRepository>();

            _logger.LogInformation(
                "Processing conversation {Id} for student {StudentId}",
                job.ConversationId, job.StudentId);

            var messages = await chatRepo.GetAllMessagesAsync(job.ConversationId, ct);
            var transcript = BuildTranscript(messages);

            var reportText = await reportService.AnalyzeConversationAsync(transcript, ct);

            await chatRepo.UpsertReportAsync(new ChatAnalysisReport
            {
                ConversationId = job.ConversationId,
                StudentId = job.StudentId,
                Title = job.Subject,
                ReportText = reportText,
                AnalyzedMessageCount = messages.Count
            }, ct);

            var recipientIds = await CollectRecipientIdsAsync(job.StudentId, studentRepo, ct);
            var EnabledIds = await NotificationPreferenceService.GetUsersWithPreferenceEnabledAsync(recipientIds,NotificationType.AiProgressReport, ct);
            await SendNotificationsAsync(job, EnabledIds, notificationRepo, notificationService, deviceTokenRepo, ct);

            _logger.LogInformation("Completed analysis for conversation {Id}", job.ConversationId);
        }

        private static string BuildTranscript(IReadOnlyList<ChatMessage> messages)
        {
            var sb = new StringBuilder();
            foreach (var msg in messages)
            {
                var role = msg.Role == MessageRole.Student ? "USER" : "AI";
                sb.AppendLine($"{role}: {msg.Content}");
            }
            return sb.ToString();
        }

        private static async Task<List<Guid>> CollectRecipientIdsAsync(
            Guid studentId,
            IStudentRepository studentRepo,
            CancellationToken ct)
        {
            var recipients = new List<Guid> { studentId };

            var parentIds = await studentRepo.GetParentIdsByStudentIdAsync(studentId, ct);
            recipients.AddRange(parentIds);

            var teacherIds = await studentRepo.GetTeacherIdsByStudentIdAsync(studentId, ct);
            recipients.AddRange(teacherIds);
               
            return recipients;
        }

        private async Task SendNotificationsAsync(
            ChatAnalysisJob job,
            List<Guid> recipientIds,
            INotificationRepository notificationRepo,
            INotificationService notificationService,
            IDeviceTokenRepository deviceTokenRepo,
            CancellationToken ct)
        {
            var notifications = recipientIds.Select(userId => new Notification
            {
                UserId = userId,
                Message = userId == job.StudentId
                    ? $"Your {job.Subject} chat analysis is ready!"
                    : $"New {job.Subject} progress report available.",
                NotificationType = NotificationType.AiProgressReport,
            }).ToList();

            await notificationRepo.AddRangeAsync(notifications, ct);

            foreach (var notification in notifications)
            {
                try
                {
                    var tokens = await deviceTokenRepo.GetTokensByUserIdAsync(notification.UserId, ct);
                    if (tokens.Count > 0)
                    {
                        await notificationService.SendToTokenAsync(
                            tokens,
                            "New Progress Report",
                            notification.Message,
                            new Dictionary<string, string>
                            {
                                ["type"] = "ai_progress_report",
                                ["conversationId"] = job.ConversationId.ToString(),
                                ["studentId"] = job.StudentId.ToString()
                            },
                            ct);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "FCM push failed for user {UserId}, in-app notification was saved.",
                        notification.UserId);
                }
            }
        }
    }
}
