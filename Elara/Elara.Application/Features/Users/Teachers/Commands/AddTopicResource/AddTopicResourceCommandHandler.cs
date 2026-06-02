using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Users.Teachers.Queries.GetTopicResources;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddTopicResource
{
    public class AddTopicResourceCommandHandler : IRequestHandler<AddTopicResourceCommand, ResourceItemDto>
    {
        private readonly IAsyncRepository<TopicResource, int> _resourceRepository;
        private readonly IAsyncRepository<Topic, int> _topicRepository;
        private readonly IFileStorageService _fileStorageService;

        public AddTopicResourceCommandHandler(
            IAsyncRepository<TopicResource, int> resourceRepository,
            IAsyncRepository<Topic, int> topicRepository,
            IFileStorageService fileStorageService)
        {
            _resourceRepository = resourceRepository;
            _topicRepository = topicRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<ResourceItemDto> Handle(AddTopicResourceCommand request, CancellationToken cancellationToken)
        {
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
            if (topic == null)
            {
                throw new KeyNotFoundException($"Topic with id {request.TopicId} not found.");
            }

            if (request.File == null || request.File.Length == 0)
            {
                throw new InvalidOperationException("Please provide a valid file.");
            }

            string url;
            try
            {
                using var stream = request.File.OpenReadStream();
                var uploadResult = await _fileStorageService.UploadAsync(stream, request.File.FileName, request.File.ContentType, cancellationToken);
                url = uploadResult.Url;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to upload file: {ex.Message}");
            }

            // Calculate size for display
            string sizeOrDuration = "";
            if (request.ResourceType == ResourceType.Pdf || request.ResourceType == ResourceType.Image)
            {
                var sizeInMb = request.File.Length / (1024.0 * 1024.0);
                sizeOrDuration = $"{sizeInMb:F1} MB";
            }

            var resource = new TopicResource
            {
                Title = request.Title,
                TopicId = request.TopicId,
                Url = url,
                ResourceType = request.ResourceType,
                SizeOrDurationText = sizeOrDuration
            };

            resource = await _resourceRepository.AddAsync(resource, cancellationToken);

            var dto = new ResourceItemDto
            {
                Id = resource.Id,
                Title = resource.Title,
                Url = resource.Url,
                Type = resource.ResourceType.ToString().ToLower(),
                SizeOrDuration = resource.SizeOrDurationText,
                ThumbnailUrl = resource.ThumbnailUrl
            };

            return dto;
        }
    }
}
