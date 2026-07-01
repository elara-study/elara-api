using Elara.Application.Common.Gamification;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetParentChildren
{
    public class GetParentChildrenQueryHandler : IRequestHandler<GetParentChildrenQuery, ParentChildrenDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IAsyncRepository<StudentParent, int> _studentParentRepository;
        private readonly IAsyncRepository<QuizSession, int> _quizSessionRepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetParentChildrenQueryHandler(
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            IAsyncRepository<StudentParent, int> studentParentRepository,
            IAsyncRepository<QuizSession, int> quizSessionRepository,
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _studentParentRepository = studentParentRepository;
            _quizSessionRepository = quizSessionRepository;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<ParentChildrenDto> Handle(GetParentChildrenQuery request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            var response = new ParentChildrenDto();

            var parentIdStr = parentId.ToString();
            var relations = await _studentRepository.GetParentChildrenWithStatsAsync(parentIdStr, cancellationToken);

            // 1. Pending Requests
            var pendingRelations = relations.Where(sp => sp.Status == StudentParentRelationStatus.Pending).ToList();
            if (pendingRelations.Any())
            {
                var pendingStudentIds = pendingRelations.Select(sp => sp.StudentId).ToList();
                var pendingProfiles = await _identityService.GetUserProfilesByIdsAsync(pendingStudentIds, cancellationToken);

                foreach (var link in pendingRelations)
                {
                    var profile = pendingProfiles.GetValueOrDefault(link.StudentId);
                    var childName = profile?.Name ?? "Child";

                    response.pending_requests.Add(new PendingRequestDto
                    {
                        request_id = link.Id.ToString(),
                        child_id = link.StudentId.ToString(),
                        name = childName,
                        requested_at = GetTimeAgo(link.CreatedAt)
                    });
                }
            }

            // 2. Active Children
            var activeRelations = relations.Where(sp => sp.Status == StudentParentRelationStatus.Accepted).ToList();
            if (activeRelations.Any())
            {
                var activeStudentIds = activeRelations.Select(sp => sp.StudentId).ToList();
                var profiles = await _identityService.GetUserProfilesByIdsAsync(activeStudentIds, cancellationToken);

                var realProgressMap = await _studentRepository.GetRealSubjectProgressForStudentsAsync(activeStudentIds, cancellationToken);

                foreach (var relation in activeRelations)
                {
                    var profile = profiles.GetValueOrDefault(relation.StudentId);
                    var name = profile?.Name ?? relation.StudentId.ToString();

                    // Get gamification level
                    StudentGamification.GetRemainingXpToNextLevel(
                        relation.TotalXP,
                        out var currentLevel,
                        out _,
                        out _);

                    var lessonsCompleted = relation.CompletedLessonsCount;

                    var realSubjectProgress = realProgressMap.GetValueOrDefault(relation.StudentId) ?? new List<ChildSubjectProgressDto>();

                    response.my_children.Add(new MyChildDto
                    {
                        id = relation.StudentId.ToString(),
                        name = name,
                        grade = (int)relation.GradeLevel,
                        level = currentLevel,
                        stats = new ChildStatsDto
                        {
                            day_streak = relation.CurrentStreak,
                            total_xp = relation.TotalXP,
                            lessons_completed = lessonsCompleted
                        },
                        subject_progress = realSubjectProgress
                    });
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
