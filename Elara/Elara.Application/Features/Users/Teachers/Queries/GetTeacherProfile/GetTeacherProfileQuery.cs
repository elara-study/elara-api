using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherProfile
{
    public class GetTeacherProfileQuery : IRequest<TeacherProfileDto>
    {
        public GetTeacherProfileQuery(Guid teacherId)
        {
            TeacherId = teacherId;
        }

        public Guid TeacherId { get; }
    }
}
