using Elara.Application.Contracts.persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Notifications.Commands.MarkBatchAsRead
{
    public class MarkBatchAsReadCommandHandler : IRequestHandler<MarkBatchAsReadCommand>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkBatchAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(MarkBatchAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationRepository.MarkBatchAsReadAsync(request.NotificationIds, request.UserId, cancellationToken);
        }
    }
}
