namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps
{
    public class TeacherRoadmapListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Grade { get; set; }
        public string Subject { get; set; } = string.Empty;
        public int ModulesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
