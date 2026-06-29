using Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetGroupRoadmap
{
    public class GetGroupRoadmapQuery : IRequest<TeacherRoadmapDetailDto>
    {
        public Guid ClassId { get; }

        public GetGroupRoadmapQuery(Guid classId)
        {
            ClassId = classId;
        }
    }
}
