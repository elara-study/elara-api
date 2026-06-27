using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps
{
    public class GetTeacherRoadmapDetailQuery : IRequest<TeacherRoadmapDetailDto>
    {
        public int RoadmapId { get; }

        public GetTeacherRoadmapDetailQuery(int roadmapId)
        {
            RoadmapId = roadmapId;
        }
    }
}
