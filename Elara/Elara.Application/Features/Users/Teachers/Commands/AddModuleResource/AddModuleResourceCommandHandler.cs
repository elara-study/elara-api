using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Users.Teachers.Queries.GetModuleResources;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddModuleResource
{
    public class AddModuleResourceCommandHandler : IRequestHandler<AddModuleResourceCommand, ResourceItemDto>
    {
        private readonly IAsyncRepository<ModuleResource, int> _resourceRepository;
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IFileStorageService _fileStorageService;

        public AddModuleResourceCommandHandler(
            IAsyncRepository<ModuleResource, int> resourceRepository,
            IAsyncRepository<Module, int> moduleRepository,
            IFileStorageService fileStorageService)
        {
            _resourceRepository = resourceRepository;
            _moduleRepository = moduleRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<ResourceItemDto> Handle(AddModuleResourceCommand request, CancellationToken cancellationToken)
        {
            var module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);

            if (module == null)
            {
                throw new KeyNotFoundException($"Module with id {request.ModuleId} not found.");
            }

            if (request.File == null || request.File.Length == 0)
            {
                throw new InvalidOperationException("Please provide a valid file.");
            }

            string url;
            string publicId;
            try
            {
                using var stream = request.File.OpenReadStream();
                var uploadResult = await _fileStorageService.UploadAsync(stream, request.File.FileName, request.File.ContentType, cancellationToken);
                url = uploadResult.Url;
                publicId = uploadResult.PublicId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to upload file.", ex);
            }

            string sizeOrDuration = "";
            if (request.ResourceType == ResourceType.Pdf || request.ResourceType == ResourceType.Image)
            {
                var sizeInMb = request.File.Length / (1024.0 * 1024.0);
                sizeOrDuration = $"{sizeInMb:F1} MB";
            }

            var resource = new ModuleResource
            {
                Title = request.Title,
                ModuleId = request.ModuleId,
                Url = url,
                PublicId = publicId,
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
