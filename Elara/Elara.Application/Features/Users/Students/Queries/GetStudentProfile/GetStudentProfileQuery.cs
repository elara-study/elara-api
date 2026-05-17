using MediatR;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentProfile
{
    public class GetStudentProfileQuery : IRequest<StudentProfileDto>
    {
        public GetStudentProfileQuery(Guid studentId)
        {
            StudentId = studentId;
        }

        public Guid StudentId { get; }
    }
}
