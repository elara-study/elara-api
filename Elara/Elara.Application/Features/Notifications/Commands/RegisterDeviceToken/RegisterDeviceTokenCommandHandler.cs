using Elara.Application.Contracts.persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Notifications.Commands.RegisterDeviceToken
{
    public class RegisterDeviceTokenCommandHandler : IRequestHandler<RegisterDeviceTokenCommand>
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;

        public RegisterDeviceTokenCommandHandler(IDeviceTokenRepository deviceTokenRepository)
        {
            _deviceTokenRepository = deviceTokenRepository;
        }

        public async Task Handle(RegisterDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            await _deviceTokenRepository.UpsertAsync(request.UserId, request.Token, cancellationToken);
        }
    }
}
