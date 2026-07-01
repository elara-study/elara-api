namespace Elara.Application.Features.Rewards.DTOs
{
    public class RewardSummaryDto
    {
        public int TotalXp { get; set; }
        public int LessonsCompleted { get; set; }
        public int StreakDays { get; set; }
        public BadgesCountDto Badges { get; set; } = new();
    }

    public class BadgesCountDto
    {
        public int Earned { get; set; }
        public int Total { get; set; }
    }
}
