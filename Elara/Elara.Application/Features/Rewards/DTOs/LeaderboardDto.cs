namespace Elara.Application.Features.Rewards.DTOs
{
    public class LeaderboardDto
    {
        public List<LeaderboardStudentDto> Leaderboard { get; set; } = new();
    }

    public class LeaderboardStudentDto
    {
        public int Rank { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int Xp { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
