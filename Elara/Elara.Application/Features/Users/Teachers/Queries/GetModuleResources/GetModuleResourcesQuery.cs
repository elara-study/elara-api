using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetModuleResources
{
    public class GetModuleResourcesQuery : IRequest<ModuleResourcesDto>
    {
        public int ModuleId { get; set; }
    }
}
