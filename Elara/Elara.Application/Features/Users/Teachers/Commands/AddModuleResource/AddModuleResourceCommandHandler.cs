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
            var module = (await _moduleRepository.FindAsync(m => m.PublicId == request.ModuleId, cancellationToken)).FirstOrDefault();

            if (module == null)
            {
                throw new KeyNotFoundException($"Module not found.");
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
                throw new InvalidOperationException($"Failed to upload file: {ex.Message}", ex);
            }

            var resourceType = request.File.ContentType switch
            {
                var ct when ct.StartsWith("video/") => ResourceType.Video,
                var ct when ct.StartsWith("image/") => ResourceType.Image,
                var ct when ct.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) => ResourceType.Pdf,
                _ => throw new InvalidOperationException($"Unsupported file type: {request.File.ContentType}")
            };

            var resource = new ModuleResource
            {
                Title = request.Title,
                ModuleId = module.Id,
                Url = url,
                CloudId = publicId,
                ResourceType = resourceType
            };

            resource = await _resourceRepository.AddAsync(resource, cancellationToken);

            var dto = new ResourceItemDto
            {
                Id = resource.Id,
                Title = resource.Title,
                Url = resource.Url
            };

            return dto;
        }
    }
}
