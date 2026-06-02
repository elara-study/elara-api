using Elara.Application.Features.Users.Teachers.Queries.GetTopicResources;
using Elara.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Elara.Application.Features.Users.Teachers.Commands.AddTopicResource
{
    public class AddTopicResourceCommand : IRequest<ResourceItemDto>
    {
        public AddTopicResourceCommand(int topicId, string title, ResourceType resourceType, IFormFile file)
        {
            TopicId = topicId;
            Title = title;
            ResourceType = resourceType;
            File = file;
        }

        public int TopicId { get; }
        public string Title { get; }
        public ResourceType ResourceType { get; }
        public IFormFile File { get; }
    }
}
