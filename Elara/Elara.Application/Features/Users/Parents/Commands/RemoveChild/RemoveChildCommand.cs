using MediatR;

namespace Elara.Application.Features.Users.Parents.Commands.RemoveChild
{
    public class RemoveChildCommand : IRequest<bool>
    {
        public Guid ChildId { get; set; }

        public RemoveChildCommand(Guid childId)
        {
            ChildId = childId;
        }
    }
}
