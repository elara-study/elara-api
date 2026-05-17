using Elara.Application.Common.Gamification;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentProfile
{
    public class GetStudentProfileQueryHandler : IRequestHandler<GetStudentProfileQuery, StudentProfileDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IAsyncRepository<QuizSession, int> _quizSessionRepository;

        public GetStudentProfileQueryHandler(
            IStudentRepository studentRepository,
            IAsyncRepository<QuizSession, int> quizSessionRepository)
        {
            _studentRepository = studentRepository;
            _quizSessionRepository = quizSessionRepository;
        }

        public async Task<StudentProfileDto> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _studentRepository.GetStudentProfileAsync(request.StudentId, cancellationToken);
            if (profile == null)
            {
                throw new KeyNotFoundException("Student not found.");
            }

            var quizzesCompleted = await _quizSessionRepository.CountAsync(
                s => s.StudentId == request.StudentId && s.Status == QuizSessionStatus.Completed,
                cancellationToken);

            var remainingToNext = StudentGamification.GetRemainingXpToNextLevel(
                profile.TotalXP,
                out var currentLevel,
                out var nextLevel,
                out var xpTarget);

            return new StudentProfileDto
            {
                User = new StudentProfileUserDto
                {
                    Id = profile.StudentId,
                    Username = profile.Username,
                    FullName = profile.FullName,
                    GradeLevel = FormatGradeLevel(profile.GradeLevel),
                    AvatarUrl = profile.AvatarUrl
                },
                Gamification = new StudentProfileGamificationDto
                {
                    Level = new StudentProfileLevelDto
                    {
                        Current = currentLevel,
                        Next = nextLevel
                    },
                    Xp = new StudentProfileXpDto
                    {
                        Current = profile.TotalXP,
                        Target = xpTarget,
                        RemainingToNextLevel = remainingToNext
                    },
                    Statistics = new StudentProfileStatisticsDto
                    {
                        DayStreak = profile.CurrentStreak,
                        TotalXp = profile.TotalXP,
                        QuizzesCompleted = quizzesCompleted
                    }
                },
                Parents = profile.Parents.Select(p => new StudentProfileParentDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    AvatarUrl = p.AvatarUrl
                }).ToList(),
                RecentAchievements = profile.RecentAchievements.Select(a => new StudentProfileAchievementDto
                {
                    Id = a.Id,
                    Title = a.Title
                }).ToList()
            };
        }

        private static string FormatGradeLevel(GradeLevel gradeLevel) =>
            $"Grade {(int)gradeLevel}";
    }
}
