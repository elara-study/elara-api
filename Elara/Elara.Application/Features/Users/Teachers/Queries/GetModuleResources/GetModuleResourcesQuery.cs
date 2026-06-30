using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetModuleResources
{
    public class GetModuleResourcesQuery : IRequest<ModuleResourcesDto>
    {
        public Guid ModuleId { get; set; }
    }
}
