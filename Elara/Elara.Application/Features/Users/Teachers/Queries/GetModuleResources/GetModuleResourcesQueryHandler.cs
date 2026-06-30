using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetModuleResources
{
    public class GetModuleResourcesQueryHandler : IRequestHandler<GetModuleResourcesQuery, ModuleResourcesDto>
    {
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<ModuleResource, int> _resourceRepository;

        public GetModuleResourcesQueryHandler(
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<ModuleResource, int> resourceRepository)
        {
            _moduleRepository = moduleRepository;
            _resourceRepository = resourceRepository;
        }

        public async Task<ModuleResourcesDto> Handle(GetModuleResourcesQuery request, CancellationToken cancellationToken)
        {
            var module = (await _moduleRepository.FindAsync(m => m.PublicId == request.ModuleId, cancellationToken)).FirstOrDefault();
            if (module == null)
            {
                throw new KeyNotFoundException($"Module not found.");
            }

            var resources = await _resourceRepository.FindAsync(r => r.ModuleId == module.Id, cancellationToken);

            var dto = new ModuleResourcesDto
            {
                ModuleId = module.PublicId,
                ModuleName = module.Title
            };

            foreach (var resource in resources)
            {
                var item = new ResourceItemDto
                {
                    Id = resource.Id,
                    Title = resource.Title,
                    Url = resource.Url
                };

                if (resource.ResourceType == ResourceType.Video) dto.Videos.Add(item);
                else if (resource.ResourceType == ResourceType.Pdf) dto.Pdfs.Add(item);
                else if (resource.ResourceType == ResourceType.Image) dto.Images.Add(item);
            }

            return dto;
        }
    }
}
