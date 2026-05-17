using Elara.Application.Contracts.Persistence.Users;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherProfile
{
    public class GetTeacherProfileQueryHandler : IRequestHandler<GetTeacherProfileQuery, TeacherProfileDto>
    {
        private readonly ITeacherRepository _teacherRepository;

        public GetTeacherProfileQueryHandler(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<TeacherProfileDto> Handle(GetTeacherProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _teacherRepository.GetTeacherProfileAsync(request.TeacherId, cancellationToken);
            if (profile == null)
            {
                throw new KeyNotFoundException("Teacher not found.");
            }

            return new TeacherProfileDto
            {
                User = new TeacherProfileUserDto
                {
                    Id = profile.TeacherId,
                    Username = profile.Username,
                    FullName = profile.FullName,
                    AvatarUrl = profile.AvatarUrl,
                    Subjects = profile.Subjects
                },
                Contact = new TeacherProfileContactDto
                {
                    Email = profile.Email,
                    Phone = profile.Phone
                },
                Statistics = new TeacherProfileStatisticsDto
                {
                    TotalStudents = profile.TotalStudents,
                    ActiveGroups = profile.ActiveGroups,
                    RoadmapsCreated = profile.RoadmapsCreated,
                    LessonsPublished = profile.LessonsPublished
                }
            };
        }
    }
}
