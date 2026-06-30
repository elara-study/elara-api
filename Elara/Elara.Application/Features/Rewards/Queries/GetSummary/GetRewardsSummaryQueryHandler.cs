using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Rewards.DTOs;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Submissions;
using MediatR;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Enums;

namespace Elara.Application.Features.Rewards.Queries.GetSummary
{
    public class GetRewardsSummaryQueryHandler : IRequestHandler<GetRewardsSummaryQuery, RewardSummaryDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IAsyncRepository<Achievement, int> _achievementRepository;
        private readonly IAsyncRepository<QuizSession, int> _quizSessionRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetRewardsSummaryQueryHandler(
            IStudentRepository studentRepository,
            IAsyncRepository<Achievement, int> achievementRepository,
            IAsyncRepository<QuizSession, int> quizSessionRepository,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _achievementRepository = achievementRepository;
            _quizSessionRepository = quizSessionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<RewardSummaryDto> Handle(GetRewardsSummaryQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? throw new Exception("User must be authenticated");

            var student = await _studentRepository.GetStudentWithAchievementsAsync(userId, cancellationToken);
            if (student == null) throw new Exception("Student not found");

            // Get total badges count
            var allAchievements = await _achievementRepository.ListAllAsync(cancellationToken);
            int totalBadges = allAchievements.Count;

            // Get earned badges count
            int earnedBadges = student.StudentAchievements?.Count ?? 0;

            // Get lessons completed 
            int completedLessons = await _quizSessionRepository.CountAsync(
                s => s.StudentId == userId && s.Status == QuizSessionStatus.Completed,
                cancellationToken);

            return new RewardSummaryDto
            {
                TotalXp = student.TotalXP,
                LessonsCompleted = completedLessons,
                StreakDays = student.CurrentStreak,
                Badges = new BadgesCountDto
                {
                    Earned = earnedBadges,
                    Total = totalBadges
                }
            };
        }
    }
}
