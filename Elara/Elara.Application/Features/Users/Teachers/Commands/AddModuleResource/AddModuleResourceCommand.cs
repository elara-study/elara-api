using Elara.Application.Features.Users.Teachers.Queries.GetModuleResources;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Elara.Application.Features.Users.Teachers.Commands.AddModuleResource
{
    public class AddModuleResourceCommand : IRequest<ResourceItemDto>
    {
        public AddModuleResourceCommand(Guid moduleId, string title, IFormFile file)
        {
            ModuleId = moduleId;
            Title = title;
            File = file;
        }

        public Guid ModuleId { get; }
        public string Title { get; }
        public IFormFile File { get; }
    }
}
