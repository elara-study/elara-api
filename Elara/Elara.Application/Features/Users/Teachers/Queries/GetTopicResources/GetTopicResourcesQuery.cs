using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTopicResources
{
    public class GetTopicResourcesQuery : IRequest<TopicResourcesDto>
    {
        public int TopicId { get; set; }
    }
}
