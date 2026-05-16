namespace Elara.Application.Features.Rewards.DTOs
{
    public class LeaderboardDto
    {
        public List<LeaderboardStudentDto> TopStudents { get; set; } = new();
        public LeaderboardStudentDto CurrentUserRank { get; set; } = new();
    }

    public class LeaderboardStudentDto
    {
        public int Rank { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Xp { get; set; }
        public int Level { get; set; }
        public bool IsCurrentUser { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
