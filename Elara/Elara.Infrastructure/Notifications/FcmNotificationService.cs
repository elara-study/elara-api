using Elara.Application.Common.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace Elara.Infrastructure.Notifications
{
    public class FcmNotificationService : INotificationService
    {
        private readonly FirebaseMessaging? _messaging;
        private readonly ILogger<FcmNotificationService> _logger;

        public FcmNotificationService(ILogger<FcmNotificationService> logger)
        {
            _messaging = FirebaseMessaging.DefaultInstance;
            _logger = logger;
        }

        public async Task SendToTokenAsync(IEnumerable<string> tokens, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default)
        {
            if (_messaging == null)
            {
                _logger.LogWarning("Firebase not initialized. Skipping multicast send.");
                return;
            }

            var message = new MulticastMessage
            {
                Tokens = tokens.ToList(),
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };
            var response = await _messaging.SendEachForMulticastAsync(message, ct);
            _logger.LogInformation("FCM multicast sent: {SuccessCount}/{Total} successful",
                response.SuccessCount, tokens.Count());
        }

        public async Task SendToTopicAsync(string topic, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default)
        {
            if (_messaging == null)
            {
                _logger.LogWarning("Firebase not initialized. Skipping topic send.");
                return;
            }

            var message = new Message
            {
                Topic = topic,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };
            var response = await _messaging.SendAsync(message, ct);
            _logger.LogInformation("FCM topic message sent: {Response}", response);
        }
    }
}
