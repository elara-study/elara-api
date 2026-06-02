using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTopicResources
{
    public class GetTopicResourcesQueryHandler : IRequestHandler<GetTopicResourcesQuery, TopicResourcesDto>
    {
        private readonly IAsyncRepository<Topic, int> _topicRepository;
        private readonly IAsyncRepository<TopicResource, int> _resourceRepository;

        public GetTopicResourcesQueryHandler(
            IAsyncRepository<Topic, int> topicRepository,
            IAsyncRepository<TopicResource, int> resourceRepository)
        {
            _topicRepository = topicRepository;
            _resourceRepository = resourceRepository;
        }

        public async Task<TopicResourcesDto> Handle(GetTopicResourcesQuery request, CancellationToken cancellationToken)
        {
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
            if (topic == null)
            {
                throw new KeyNotFoundException($"Topic with id {request.TopicId} not found.");
            }

            var topicResources = _resourceRepository.AsQueryable()
                .Where(r => r.TopicId == request.TopicId)
                .ToList();

            var dto = new TopicResourcesDto
            {
                TopicId = topic.Id,
                TopicName = topic.Title
            };

            foreach (var resource in topicResources)
            {
                var item = new ResourceItemDto
                {
                    Id = resource.Id,
                    Title = resource.Title,
                    Url = resource.Url,
                    Type = resource.ResourceType.ToString().ToLower(),
                    ThumbnailUrl = resource.ThumbnailUrl,
                    SizeOrDuration = resource.SizeOrDurationText
                };

                if (resource.ResourceType == ResourceType.Video) dto.Videos.Add(item);
                else if (resource.ResourceType == ResourceType.Pdf) dto.Pdfs.Add(item);
                else if (resource.ResourceType == ResourceType.Image) dto.Images.Add(item);
            }

            return dto;
        }
    }
}
