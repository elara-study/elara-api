using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.JunctionTables;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherHome
{
    public class GetTeacherHomeQueryHandler : IRequestHandler<GetTeacherHomeQuery, TeacherHomeDto>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetTeacherHomeQueryHandler(
            ITeacherRepository teacherRepository,
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _teacherRepository = teacherRepository;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<TeacherHomeDto> Handle(GetTeacherHomeQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User must be authenticated");

            var response = new TeacherHomeDto();

            var fullName = await _identityService.GetUserNameByIdAsync(teacherId);
            response.FirstName = string.IsNullOrWhiteSpace(fullName)
                 ? "Teacher"
                 : fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "Teacher";

            var classesData = await _teacherRepository.GetClassesByTeacherAsync(teacherId, cancellationToken);
            response.Groups = classesData.Select(c => new TeacherGroupDto
            {
                Id = c.PublicId.ToString(),
                Name = c.ClassName,
                StudentsCount = c.StudentClasses?.Count ?? 0
            }).ToList();

            var roadmapsData = await _teacherRepository.GetRoadmapsByTeacherAsync(teacherId, cancellationToken);
            response.Roadmaps = roadmapsData.Select(r => new TeacherRoadmapDto
            {
                Id = r.Id.ToString(),
                Title = r.Name,
                Subject = r.Subject?.Name ?? "",
                LessonsCount = r.Modules?.SelectMany(t => t.Homeworks).Count() ?? 0,
                Grade = (int)r.Grade
            }).ToList();

            var activeStudents = classesData
                .SelectMany(c => c.StudentClasses ?? new List<StudentClass>())
                .Select(sc => sc.StudentId)
                .Distinct()
                .Count();

            response.Stats.ActiveStudents = new ActiveStudentsStatDto
            {
                Count = activeStudents,
                Delta = 0
            };

            var avgCompletion = await _teacherRepository.GetAvgCompletionByTeacherAsync(teacherId, cancellationToken);
            response.Stats.AvgCompletion = new AvgCompletionStatDto
            {
                Percentage = (int)Math.Round(avgCompletion),
                Delta = 0
            };

            var pendingTop5 = await _teacherRepository.GetPendingSubmissionsAsync(teacherId, 5, cancellationToken);
            var recentTop5 = await _teacherRepository.GetRecentSubmissionsAsync(teacherId, 5, cancellationToken);

            var allStudentIds = pendingTop5.Select(p => p.StudentId)
               .Concat(recentTop5.Select(r => r.StudentId))
               .Distinct()
               .ToList();

            var studentProfiles = await _identityService.GetUserProfilesByIdsAsync(allStudentIds, cancellationToken);
            var userImages = await _identityService.GetUserImagesByIdsAsync(allStudentIds, cancellationToken);

            foreach (var pending in pendingTop5)
            {
                var profile = studentProfiles.GetValueOrDefault(pending.StudentId);
                response.PendingTasks.Add(new TeacherPendingTaskDto
                {
                    Id = pending.Id.ToString(),
                    Title = pending.Homework?.Title ?? "Homework",
                    Meta = $"Submission from {profile?.Name ?? "Unknown Student"}",
                    Type = "rating"
                });
            }

            foreach (var recent in recentTop5)
            {
                var profile = studentProfiles.GetValueOrDefault(recent.StudentId);
                var imageUrl = userImages.GetValueOrDefault(recent.StudentId, string.Empty);

                response.RecentActivity.Add(new TeacherRecentActivityDto
                {
                    Student = new ActivityStudentDto
                    {
                        Username = profile?.Username ?? "",
                        Name = profile?.Name ?? "Unknown Student",
                        Avatar = imageUrl
                    },
                    Type = "homework_submitted",
                    TargetId = recent.Id.ToString(),
                    Date = recent.CreatedAt.ToString("o")
                });
            }

            return response;
        }
    }
}
