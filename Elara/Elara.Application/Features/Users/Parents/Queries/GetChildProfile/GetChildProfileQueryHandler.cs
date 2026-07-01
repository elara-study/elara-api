using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Common.Gamification;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetChildProfile
{
    public class GetChildProfileQueryHandler : IRequestHandler<GetChildProfileQuery, ChildProfileDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IAsyncRepository<StudentClass, int> _studentClassRepository;
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly IAsyncRepository<Elara.Domain.Entities.Educational.Subject, int> _subjectRepository;
        private readonly IAsyncRepository<QuizSession, int> _quizSessionRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetChildProfileQueryHandler(
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            IAsyncRepository<StudentClass, int> studentClassRepository,
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            IAsyncRepository<Elara.Domain.Entities.Educational.Subject, int> subjectRepository,
            IAsyncRepository<QuizSession, int> quizSessionRepository,
            IChatRepository chatRepository,
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _studentClassRepository = studentClassRepository;
            _moduleRepository = moduleRepository;
            _homeworkRepository = homeworkRepository;
            _subjectRepository = subjectRepository;
            _quizSessionRepository = quizSessionRepository;
            _chatRepository = chatRepository;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<ChildProfileDto> Handle(GetChildProfileQuery request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            // Verify relationship
            var isParent = await _studentRepository.IsParentOfStudentAsync(parentId, request.ChildId, cancellationToken);
            if (!isParent)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this child's profile.");
            }

            var student = await _studentRepository.GetByIdAsync(request.ChildId, cancellationToken);
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found.");
            }

            var profiles = await _identityService.GetUserProfilesByIdsAsync(new[] { request.ChildId }, cancellationToken);
            var images = await _identityService.GetUserImagesByIdsAsync(new[] { request.ChildId }, cancellationToken);

            var profile = profiles.GetValueOrDefault(request.ChildId);
            var avatarUrl = images.GetValueOrDefault(request.ChildId, string.Empty);
            var name = profile?.Name ?? student.Id.ToString();
            var username = profile?.Username ?? string.Empty;
            if (!string.IsNullOrEmpty(username) && !username.StartsWith("@"))
            {
                username = "@" + username;
            }

            // Gamification
            int currentLevel, nextLevel, xpTarget;
            var xpNeeded = StudentGamification.GetRemainingXpToNextLevel(student.TotalXP, out currentLevel, out nextLevel, out xpTarget);

            // Lessons count
            var studentGroups = await _classRepository.GetStudentGroupsByStudentIdAsync(request.ChildId, cancellationToken);
            var totalLessons = studentGroups.Sum(g => g.Stats.Lessons.Total);
            var lessonsCompleted = await _quizSessionRepository.CountAsync(
                s => s.StudentId == request.ChildId && s.Status == QuizSessionStatus.Completed && !s.IsDeleted,
                cancellationToken);

            var attendance = student.CurrentStreak > 0 
                ? Math.Min(100, 90 + student.CurrentStreak) 
                : 95;

            var response = new ChildProfileDto
            {
                child_info = new ChildInfoDto
                {
                    name = name,
                    username = username,
                    grade = $"Grade {(int)student.GradeLevel} Student",
                    level = currentLevel,
                    xp = new ChildXpDto
                    {
                        current = student.TotalXP,
                        target_xp = xpTarget,
                        xp_needed = xpNeeded
                    },
                    stats = new ChildProfileStatsDto
                    {
                        total_xp = student.TotalXP,
                        lessons = $"{lessonsCompleted}/{totalLessons}",
                        day_streak = student.CurrentStreak,
                        attendance_percentage = attendance
                    }
                }
            };

            // Latest Insight / Report
            var reports = await _chatRepository.GetReportsByStudentIdsAsync(new[] { request.ChildId }, cancellationToken);
            var latestReport = reports.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            if (latestReport != null)
            {
                response.latest_insight = new LatestInsightDto
                {
                    time_ago = GetTimeAgo(latestReport.CreatedAt),
                    content = latestReport.ReportText
                };
            }

            // Latest Homework
            var studentClasses = await _studentClassRepository.FindAsync(
                sc => sc.StudentId == request.ChildId && sc.IsActive && !sc.IsDeleted,
                cancellationToken);
            var classIds = studentClasses.Select(sc => sc.ClassId).ToList();

            if (classIds.Any())
            {
                var classes = await _classRepository.FindAsync(
                    c => classIds.Contains(c.Id) && !c.IsDeleted,
                    cancellationToken);
                var roadmapIds = classes.Select(c => c.RoadmapId).Where(id => id.HasValue).Select(id => id!.Value).ToList();

                if (roadmapIds.Any())
                {
                    var modules = await _moduleRepository.FindAsync(
                        m => roadmapIds.Contains(m.RoadmapId) && !m.IsDeleted,
                        cancellationToken);
                    var moduleIds = modules.Select(m => m.Id).ToList();

                    if (moduleIds.Any())
                    {
                        var homeworks = await _homeworkRepository.FindAsync(
                            h => moduleIds.Contains(h.ModuleId) && !h.IsDeleted,
                            cancellationToken);

                        var latestHomework = homeworks.OrderByDescending(h => h.CreatedAt).FirstOrDefault();
                        if (latestHomework != null)
                        {
                            var module = modules.FirstOrDefault(m => m.Id == latestHomework.ModuleId);
                            var subject = module != null ? await _subjectRepository.GetByIdAsync(module.SubjectId, cancellationToken) : null;
                            var classEntity = (module != null) ? classes.FirstOrDefault(c => c.RoadmapId == module.RoadmapId) : null;

                            response.latest_homework = new LatestHomeworkDto
                            {
                                module_name = module?.ModuleName ?? module?.Title ?? string.Empty,
                                title = latestHomework.Title,
                                subject = subject?.Name ?? string.Empty,
                                @class = classEntity?.ClassName ?? string.Empty
                            };
                        }
                    }
                }
            }

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
