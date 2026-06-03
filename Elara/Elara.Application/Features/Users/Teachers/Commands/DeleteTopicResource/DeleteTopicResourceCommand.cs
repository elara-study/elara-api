using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteTopicResource
{
    public class DeleteTopicResourceCommand : IRequest<bool>
    {
        public int ResourceId { get; set; }
    }
}
