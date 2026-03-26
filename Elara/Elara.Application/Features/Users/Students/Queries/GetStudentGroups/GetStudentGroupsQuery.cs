using MediatR;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroups
{
    public class GetStudentGroupsQuery : IRequest<GetStudentGroupsResponse>
    {
        public GetStudentGroupsQuery(Guid studentId)
        {
            StudentId = studentId;
        }

        public Guid StudentId { get; }
    }
}
