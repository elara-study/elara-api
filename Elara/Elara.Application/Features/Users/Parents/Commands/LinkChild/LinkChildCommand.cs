using MediatR;

namespace Elara.Application.Features.Users.Parents.Commands.LinkChild
{
    public class LinkChildCommand : IRequest<bool>
    {
        public string child_username { get; set; } = string.Empty;
    }
}
