using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherHome
{
    public class GetTeacherHomeQueryHandler : IRequestHandler<GetTeacherHomeQuery, TeacherHomeDto>
    {
        private readonly IAsyncRepository<Class, int> _classRepository;
        private readonly IAsyncRepository<Roadmap, int> _roadmapRepository;
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetTeacherHomeQueryHandler(
            IAsyncRepository<Class, int> classRepository,
            IAsyncRepository<Roadmap, int> roadmapRepository,
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _roadmapRepository = roadmapRepository;
            _submissionRepository = submissionRepository;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<TeacherHomeDto> Handle(GetTeacherHomeQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User must be authenticated");
          
            var response = new TeacherHomeDto();

            // First Name
            var fullName = await _identityService.GetUserNameByIdAsync(teacherId);
            response.FirstName = fullName?.Split(' ').FirstOrDefault() ?? "Teacher";

            // Groups
            var classesData = await _classRepository.FindAsync(
            c => c.TeacherId == teacherId, cancellationToken);

            response.Groups = classesData.Select(c => new TeacherGroupDto
            {
                Id = c.PublicId.ToString(),
                Name = c.ClassName,
                StudentsCount = c.StudentClasses?.Count ?? 0
            }).ToList();

            // Roadmaps
            var roadmapsData = await _roadmapRepository.FindAsync(
                r => r.TeacherId == teacherId, cancellationToken);

            response.Roadmaps = roadmapsData.Select(r => new TeacherRoadmapDto
            {
                Id = r.Id.ToString(),
                Title = r.Name,
                Subject = r.Subject?.Name ?? "",
                LessonsCount = r.Topics?.SelectMany(t => t.Lessons).Count() ?? 0,
                Grade = (int)r.Grade
            }).ToList();

            // Stats - Active Students
            var allClasses = await _classRepository.FindAsync(
            c => c.TeacherId == teacherId, cancellationToken);

            var activeStudents = allClasses
                .SelectMany(c => c.StudentClasses ?? new List<StudentClass>())
                .Select(sc => sc.StudentId)
                .Distinct()
                .Count();

            response.Stats.ActiveStudents = new ActiveStudentsStatDto
            {
                Count = activeStudents,
                Delta = 0
            };

            // Stats - Average Completion
            var submissions = await _submissionRepository.FindAsync(
                s => s.Assignment.TeacherId == teacherId && s.Assignment.MaxScore > 0, cancellationToken);

            int avgCompletion = 0;
            if (submissions.Any())
            {
                var avg = submissions.Average(s => (s.Score / s.Assignment.MaxScore) * 100);
                avgCompletion = (int)Math.Round(avg);
            }

            response.Stats.AvgCompletion = new AvgCompletionStatDto
            {
                Percentage = avgCompletion,
                Delta = 0
            };

            // Pending Tasks
            var pendingTasksData = await _submissionRepository.FindAsync(
              s => s.Assignment.TeacherId == teacherId &&
                   s.Score == 0 &&
                   string.IsNullOrEmpty(s.TeacherFeedback),
              cancellationToken);

            var pendingTop5 = pendingTasksData
            .OrderBy(s => s.CreatedAt)
            .Take(5)
            .ToList();

            // Recent Activity
            var recentData = await _submissionRepository.FindAsync(
            s => s.Assignment.TeacherId == teacherId, cancellationToken);

            var recentTop5 = recentData
                .OrderByDescending(s => s.CreatedAt)
                .Take(5)
                .ToList();

            var allStudentIds = pendingTop5.Select(p => p.StudentId)
               .Concat(recentTop5.Select(r => r.StudentId))
               .Distinct()
               .ToList();

            var studentNames = await _identityService.GetUserNamesByIdsAsync(allStudentIds, cancellationToken);
            foreach (var pending in pendingTop5)
            {
                var name = studentNames.GetValueOrDefault(pending.StudentId, "Unknown Student");
                response.PendingTasks.Add(new TeacherPendingTaskDto
                {
                    Id = pending.Id.ToString(),
                    Title = pending.Assignment?.Title ?? "Homework",
                    Meta = $"Submission from {name}",
                    Type = "rating"
                });
            }

            var userImages = await _identityService.GetUserImagesByIdsAsync(allStudentIds, cancellationToken);
            foreach (var recent in recentTop5)
            {
                var name = studentNames.GetValueOrDefault(recent.StudentId, "Unknown Student");
                var imageUrl = userImages.GetValueOrDefault(recent.StudentId, string.Empty);

                response.RecentActivity.Add(new TeacherRecentActivityDto
                {
                    Student = new ActivityStudentDto
                    {
                        Username = name.Replace(" ", "").ToLower(),
                        Name = name,
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
