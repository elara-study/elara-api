using Elara.Application.Contracts.persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationRepository.MarkAsReadAsync(request.NotificationId, request.UserId, cancellationToken);
        }
    }
}
