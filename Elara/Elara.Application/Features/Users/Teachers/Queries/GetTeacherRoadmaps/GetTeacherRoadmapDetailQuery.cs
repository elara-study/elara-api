using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps
{
    public class GetTeacherRoadmapDetailQuery : IRequest<TeacherRoadmapDetailDto>
    {
        public Guid RoadmapId { get; }

        public GetTeacherRoadmapDetailQuery(Guid roadmapId)
        {
            RoadmapId = roadmapId;
        }
    }
}
