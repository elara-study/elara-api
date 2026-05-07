using Elara.Application.Contracts.persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Notifications.Commands.RemoveDeviceToken
{
    public class RemoveDeviceTokenCommandHandler : IRequestHandler<RemoveDeviceTokenCommand>
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;

        public RemoveDeviceTokenCommandHandler(IDeviceTokenRepository deviceTokenRepository)
        {
            _deviceTokenRepository = deviceTokenRepository;
        }

        public async Task Handle(RemoveDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            await _deviceTokenRepository.RemoveByTokenAsync(request.Token, cancellationToken);
        }
    }
}
