using Elara.Application.Common.Gamification;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Features.Users.Students.Queries.GetStudentGroups;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetStudentDetail
{
    public class GetStudentDetailQueryHandler : IRequestHandler<GetStudentDetailQuery, StudentDetailResponse>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IAsyncRepository<QuizSession, int> _quizSessionRepository;
        private readonly IAsyncRepository<TeacherInsight, int> _teacherInsightRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetStudentDetailQueryHandler(
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            IChatRepository chatRepository,
            IAsyncRepository<QuizSession, int> quizSessionRepository,
            IAsyncRepository<TeacherInsight, int> teacherInsightRepository,
            ICurrentUserService currentUserService)
        {
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _chatRepository = chatRepository;
            _quizSessionRepository = quizSessionRepository;
            _teacherInsightRepository = teacherInsightRepository;
            _currentUserService = currentUserService;
        }

        public async Task<StudentDetailResponse> Handle(GetStudentDetailQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var teacher = await _teacherRepository.GetTeacherWithStudentsAsync(teacherId, cancellationToken)
                ?? throw new KeyNotFoundException("Teacher not found.");

            var hasRelationship = teacher.StudentTeachers.Any(st => st.StudentId == request.StudentId);
            if (!hasRelationship)
                throw new KeyNotFoundException("Student not found.");

            var profile = await _studentRepository.GetStudentProfileAsync(request.StudentId, cancellationToken)
                ?? throw new KeyNotFoundException("Student not found.");

            var lessonsCompleted = await _quizSessionRepository.CountAsync(
                s => s.StudentId == request.StudentId && s.Status == QuizSessionStatus.Completed,
                cancellationToken);

            var studentGroups = await _classRepository.GetStudentGroupsByStudentIdAsync(request.StudentId, cancellationToken);
            var totalLessons = studentGroups.Sum(g => g.Stats.Lessons.Total);

            var remainingToNext = StudentGamification.GetRemainingXpToNextLevel(
                profile.TotalXP,
                out var currentLevel,
                out var nextLevel,
                out var xpTarget);

            var progressPercentage = xpTarget > 0
                ? Math.Round((double)profile.TotalXP / xpTarget * 100, 2)
                : 100.0;

            var aiReports = await _chatRepository.GetReportsByStudentIdAsync(request.StudentId, cancellationToken);
            var teacherInsights = await _teacherInsightRepository.FindAsync(
                t => t.StudentId == request.StudentId && !t.IsDeleted, cancellationToken);

            var insights = new List<StudentDetailInsightDto>();
            insights.AddRange(aiReports.Select(r => new StudentDetailInsightDto
            {
                Id = r.PublicId,
                Source = "ai",
                LastUpdated = r.UpdatedAt ?? r.CreatedAt,
                Content = r.ReportText
            }));
            insights.AddRange(teacherInsights.Select(t => new StudentDetailInsightDto
            {
                Id = t.PublicId,
                Source = "teacher",
                LastUpdated = t.CreatedAt,
                Content = t.Content
            }));
            insights = insights.OrderByDescending(i => i.LastUpdated).ToList();

            return new StudentDetailResponse
            {
                User = new StudentDetailUserDto
                {
                    Id = profile.StudentId,
                    Username = profile.Username,
                    FullName = profile.FullName,
                    AvatarUrl = profile.AvatarUrl,
                    GradeLevel = $"Grade {(int)profile.GradeLevel}"
                },
                Gamification = new StudentDetailGamificationDto
                {
                    CurrentLevel = currentLevel,
                    CurrentXp = profile.TotalXP,
                    NextLevelXpThreshold = xpTarget,
                    XpToNextLevel = remainingToNext,
                    ProgressPercentage = progressPercentage
                },
                Stats = new StudentDetailStatsDto
                {
                    TotalXp = profile.TotalXP,
                    LessonsCompleted = lessonsCompleted,
                    TotalLessons = totalLessons,
                    StreakDays = profile.CurrentStreak,
                    AttendanceRate = Math.Min(1.0, profile.CurrentStreak / 30.0)
                },
                Parents = profile.Parents.Select(p => new StudentDetailParentDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    AvatarUrl = p.AvatarUrl
                }).ToList(),
                Insights = insights
            };
        }
    }
}
