using Elara.Application.Contracts.persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Notifications.Commands.MarkAllAsRead
{
    public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkAllAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationRepository.MarkAllAsReadAsync(request.UserId, cancellationToken);
        }
    }
}
