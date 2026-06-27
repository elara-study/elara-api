using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps
{
    public class GetTeacherRoadmapsQuery : IRequest<List<TeacherRoadmapListDto>>
    {
    }
}
