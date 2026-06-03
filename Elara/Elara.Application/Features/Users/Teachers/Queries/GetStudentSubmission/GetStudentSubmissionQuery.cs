using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetStudentSubmission
{
    public class GetStudentSubmissionQuery : IRequest<StudentSubmissionDetailDto>
    {
        public int SubmissionId { get; set; }
    }
}
