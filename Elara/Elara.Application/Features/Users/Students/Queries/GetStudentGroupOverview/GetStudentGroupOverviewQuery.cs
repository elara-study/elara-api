using MediatR;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroupOverview
{
    public class GetStudentGroupOverviewQuery : IRequest<GetStudentGroupOverviewResponse>
    {
        public GetStudentGroupOverviewQuery(Guid studentId, Guid groupId)
        {
            StudentId = studentId;
            GroupId = groupId;
        }

        public Guid StudentId { get; }
        public Guid GroupId { get; }
    }
}
