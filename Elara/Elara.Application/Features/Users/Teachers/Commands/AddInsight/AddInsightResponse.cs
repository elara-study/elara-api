namespace Elara.Application.Features.Users.Teachers.Commands.AddInsight
{
    public class AddInsightResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
