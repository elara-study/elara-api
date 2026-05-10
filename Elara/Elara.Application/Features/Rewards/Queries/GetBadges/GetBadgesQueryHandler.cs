using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Rewards.DTOs;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Enums;
using MediatR;

using Elara.Application.Contracts.Persistence.Users;

namespace Elara.Application.Features.Rewards.Queries.GetBadges
{
    public class GetBadgesQueryHandler : IRequestHandler<GetBadgesQuery, List<BadgeDto>>
    {
        private readonly IAsyncRepository<Achievement, int> _achievementRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAsyncRepository<QuizSession, int> _quizSessionRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetBadgesQueryHandler(
            IAsyncRepository<Achievement, int> achievementRepository,
            IStudentRepository studentRepository,
            IAsyncRepository<QuizSession, int> quizSessionRepository,
            ICurrentUserService currentUserService)
        {
            _achievementRepository = achievementRepository;
            _studentRepository = studentRepository;
            _quizSessionRepository = quizSessionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<BadgeDto>> Handle(GetBadgesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? throw new Exception("User must be authenticated");

            var student = await _studentRepository.GetStudentWithAchievementsAsync(userId, cancellationToken);
            if (student == null) throw new Exception("Student not found");

            // Load StudentAchievements
            var earnedAchievements = student.StudentAchievements?.ToList() ?? new List<Domain.Entities.JunctionTables.StudentAchievement>();

            var allAchievements = await _achievementRepository.ListAllAsync(cancellationToken);
            
            var sessions = _quizSessionRepository.AsQueryable()
                .Where(s => s.StudentId == userId && s.Status == QuizSessionStatus.Completed)
                .ToList();

            var badges = new List<BadgeDto>();

            foreach (var achievement in allAchievements)
            {
                var earnedRecord = earnedAchievements.FirstOrDefault(sa => sa.AchievementId == achievement.Id);
                bool isEarned = earnedRecord != null;

                BadgeProgressDto? progress = null;

                if (isEarned)
                {
                    progress = new BadgeProgressDto
                    {
                        Current = achievement.TargetValue,
                        Target = achievement.TargetValue,
                        Percentage = 100
                    };
                }
                else if (achievement.TargetValue > 0)
                {
                    int currentValue = 0;
                    switch (achievement.AchievementType)
                    {
                        case AchievementType.TotalXP:
                            currentValue = student.TotalXP;
                            break;
                        case AchievementType.Streak:
                            currentValue = student.CurrentStreak;
                            break;
                        case AchievementType.LessonsCompleted:
                            currentValue = sessions.Count;
                            break;
                        case AchievementType.SpecificQuizScore:
                            var hasPerfectScore = sessions.Any(s => s.WrongAnswers == 0 && s.CorrectAnswers > 0);
                            currentValue = hasPerfectScore ? achievement.TargetValue : 0;
                            break;
                        default:
                            currentValue = 0;
                            break;
                    }

                    if (currentValue > achievement.TargetValue) currentValue = achievement.TargetValue;

                    progress = new BadgeProgressDto
                    {
                        Current = currentValue,
                        Target = achievement.TargetValue,
                        Percentage = (int)((double)currentValue / achievement.TargetValue * 100)
                    };
                }

                badges.Add(new BadgeDto
                {
                    Id = achievement.Id,
                    Title = achievement.Title,
                    Description = achievement.Description,
                    ImageUrl = achievement.ImageUrl,
                    IsEarned = isEarned,
                    EarnedAt = earnedRecord?.EarnedAt,
                    Progress = progress
                });
            }

            return badges.OrderBy(b => b.IsEarned ? 0 : 1).ThenBy(b => b.Id).ToList();
        }
    }
}
