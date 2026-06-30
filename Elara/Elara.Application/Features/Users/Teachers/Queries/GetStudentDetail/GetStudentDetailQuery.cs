using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetStudentDetail
{
    public class GetStudentDetailQuery : IRequest<StudentDetailResponse>
    {
        public Guid StudentId { get; }

        public GetStudentDetailQuery(Guid studentId)
        {
            StudentId = studentId;
        }
    }
}
