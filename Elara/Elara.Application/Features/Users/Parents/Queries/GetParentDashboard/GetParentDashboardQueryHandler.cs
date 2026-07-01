using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetParentDashboard
{
    public class GetParentDashboardQueryHandler : IRequestHandler<GetParentDashboardQuery, ParentDashboardDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IAsyncRepository<QuizSession, int> _quizSessionRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetParentDashboardQueryHandler(
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<QuizSession, int> quizSessionRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            IAsyncRepository<Module, int> moduleRepository,
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _submissionRepository = submissionRepository;
            _quizSessionRepository = quizSessionRepository;
            _homeworkRepository = homeworkRepository;
            _moduleRepository = moduleRepository;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<ParentDashboardDto> Handle(GetParentDashboardQuery request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            var parentName = await _identityService.GetUserNameByIdAsync(parentId) ?? "elara";

            var response = new ParentDashboardDto
            {
                user = new UserDto { name = parentName }
            };

            // Get all accepted linked children
            var children = await _studentRepository.GetByParentIdAsync(parentId, cancellationToken);
            if (!children.Any())
            {
                response.overall_stats = new OverallStatsDto { avg_completion = 0, avg_attendance = 0 };
                return response;
            }

            var childIds = children.Select(c => c.Id).ToList();
            var profiles = await _identityService.GetUserProfilesByIdsAsync(childIds, cancellationToken);
            var images = await _identityService.GetUserImagesByIdsAsync(childIds, cancellationToken);

            var totalCompletionPercentage = 0;
            var totalAttendancePercentage = 0;

            foreach (var child in children)
            {
                var profile = profiles.GetValueOrDefault(child.Id);
                var imageUrl = images.GetValueOrDefault(child.Id, string.Empty);
                var name = profile?.Name ?? child.Id.ToString();
          
                var studentClasses = await _classRepository.GetStudentGroupsByStudentIdAsync(child.Id, cancellationToken);
                var totalLessons = studentClasses.Sum(c => c.Stats.Lessons.Total);
                
                var lessonsCompleted = await _quizSessionRepository.CountAsync(
                    s => s.StudentId == child.Id && s.Status == QuizSessionStatus.Completed && !s.IsDeleted, 
                    cancellationToken);

                var completionPercentage = totalLessons > 0 
                    ? (int)Math.Round((double)lessonsCompleted / totalLessons * 100) 
                    : 0;

                totalCompletionPercentage += completionPercentage;

                // Attendance rate based on streak
                var attendance = child.CurrentStreak > 0 
                    ? Math.Min(100, 90 + child.CurrentStreak) 
                    : 95;
                
                totalAttendancePercentage += attendance;

                response.children.Add(new ChildProgressDto
                {
                    id = child.Id.ToString(),
                    name = name,
                    avatar_url = imageUrl,
                    progress = new ProgressDto
                    {
                        current_lesson = lessonsCompleted,
                        total_lessons = totalLessons,
                        completion_percentage = completionPercentage
                    }
                });
            }

            // Overall Stats averages
            response.overall_stats = new OverallStatsDto
            {
                avg_completion = (int)Math.Round((double)totalCompletionPercentage / children.Count),
                avg_attendance = (int)Math.Round((double)totalAttendancePercentage / children.Count)
            };

            var activities = new List<RecentActivityDto>();

            var allSubmissions = await _submissionRepository.FindAsync(
                s => childIds.Contains(s.StudentId), 
                cancellationToken);

            var recentSubmissions = allSubmissions
                .OrderByDescending(s => s.CreatedAt)
                .Take(5)
                .ToList();

            if (recentSubmissions.Any())
            {
                var homeworkIds = recentSubmissions.Select(s => s.HomeworkId).Distinct().ToList();
                var homeworks = await _homeworkRepository.FindAsync(h => homeworkIds.Contains(h.Id), cancellationToken);
                var homeworkMap = homeworks.ToDictionary(h => h.Id);

                foreach (var sub in recentSubmissions)
                {
                    var profile = profiles.GetValueOrDefault(sub.StudentId);
                    var childName = profile?.Name ?? "Child";
                    var hw = homeworkMap.GetValueOrDefault(sub.HomeworkId);

                    activities.Add(new RecentActivityDto
                    {
                        id = $"act_sub_{sub.Id}",
                        type = "homework_submission",
                        title = "Homework Submission",
                        description = $"{childName} submitted {hw?.Title ?? "Homework"}",
                        time_ago = GetTimeAgo(sub.CreatedAt)
                    });
                }
            }

            var allSessions = await _quizSessionRepository.FindAsync(
                s => childIds.Contains(s.StudentId) && s.Status == QuizSessionStatus.Completed && !s.IsDeleted, 
                cancellationToken);

            var recentSessions = allSessions
                .OrderByDescending(s => s.CompletedAt ?? s.CreatedAt)
                .Take(5)
                .ToList();

            if (recentSessions.Any())
            {
                var moduleIds = recentSessions
                    .Where(s => s.ModuleId.HasValue)
                    .Select(s => s.ModuleId!.Value)
                    .Distinct()
                    .ToList();
                var modules = await _moduleRepository.FindAsync(m => moduleIds.Contains(m.Id), cancellationToken);
                var moduleMap = modules.ToDictionary(m => m.Id);

                foreach (var session in recentSessions)
                {
                    var profile = profiles.GetValueOrDefault(session.StudentId);
                    var childName = profile?.Name ?? "Child";
                    var completedTime = session.CompletedAt ?? session.CreatedAt;
                    var mod = session.ModuleId.HasValue ? moduleMap.GetValueOrDefault(session.ModuleId.Value) : null;

                    activities.Add(new RecentActivityDto
                    {
                        id = $"act_session_{session.Id}",
                        type = "lesson_completion",
                        title = "Lesson Completion",
                        description = $"{childName} completed {mod?.Title ?? "Module"}",
                        time_ago = GetTimeAgo(completedTime)
                    });
                }
            }

            response.recent_activity = activities.Take(5).ToList();

            return response;
        }

        private static string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            return $"{(int)span.TotalDays}d ago";
        }
    }
}
