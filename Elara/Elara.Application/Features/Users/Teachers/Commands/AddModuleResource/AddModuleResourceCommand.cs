using Elara.Application.Features.Users.Teachers.Queries.GetModuleResources;
using Elara.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Elara.Application.Features.Users.Teachers.Commands.AddModuleResource
{
    public class AddModuleResourceCommand : IRequest<ResourceItemDto>
    {
        public AddModuleResourceCommand(Guid moduleId, string title, ResourceType resourceType, IFormFile file)
        {
            ModuleId = moduleId;
            Title = title;
            ResourceType = resourceType;
            File = file;
        }

        public Guid ModuleId { get; }
        public string Title { get; }
        public ResourceType ResourceType { get; }
        public IFormFile File { get; }
    }
}
