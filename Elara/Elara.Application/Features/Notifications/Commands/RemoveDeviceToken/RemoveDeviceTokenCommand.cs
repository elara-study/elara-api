using MediatR;

namespace Elara.Application.Features.Notifications.Commands.RemoveDeviceToken
{
    public class RemoveDeviceTokenCommand : IRequest
    {
        public string Token { get; }

        public RemoveDeviceTokenCommand(string token)
        {
            Token = token;
        }
    }
}
