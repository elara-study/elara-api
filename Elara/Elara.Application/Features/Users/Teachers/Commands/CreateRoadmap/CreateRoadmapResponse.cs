namespace Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap
{
    public class CreateRoadmapResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Grade { get; set; }
        public int SubjectId { get; set; }
        public Guid TeacherId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
