using MediatR;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroupModules
{
    public class GetStudentGroupModulesQuery : IRequest<GetStudentGroupModulesResponse>
    {
        public Guid StudentId { get; }
        public Guid GroupId { get; }

        public GetStudentGroupModulesQuery(Guid studentId, Guid groupId)
        {
            StudentId = studentId;
            GroupId = groupId;
        }
    }
}
