using MediatR;

namespace Elara.Application.Features.Users.Parents.Commands.RespondLinkRequest
{
    public class RespondLinkRequestCommand : IRequest<bool>
    {
        public int RequestId { get; }
        public string Action { get; }

        public RespondLinkRequestCommand(int requestId, string action)
        {
            RequestId = requestId;
            Action = action;
        }
    }
}
