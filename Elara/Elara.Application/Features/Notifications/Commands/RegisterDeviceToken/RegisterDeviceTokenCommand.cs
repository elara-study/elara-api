using MediatR;

namespace Elara.Application.Features.Notifications.Commands.RegisterDeviceToken
{
    public class RegisterDeviceTokenCommand : IRequest
    {
        public Guid UserId { get; }
        public string Token { get; }

        public RegisterDeviceTokenCommand(Guid userId, string token)
        {
            UserId = userId;
            Token = token;
        }
    }
}
