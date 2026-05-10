namespace Elara.Application.Features.Rewards.DTOs
{
    public class BadgeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsEarned { get; set; }
        public DateTime? EarnedAt { get; set; }
        public BadgeProgressDto? Progress { get; set; }
    }

    public class BadgeProgressDto
    {
        public int Current { get; set; }
        public int Target { get; set; }
        public int Percentage { get; set; }
    }
}
