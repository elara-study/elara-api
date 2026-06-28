using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetModuleHomeworks
{
    public class GetModuleHomeworksQuery : IRequest<List<ModuleHomeworkDto>>
    {
        public int RoadmapId { get; set; }
        public int ModuleId { get; set; }
    }
}
