namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherRoadmaps
{
    public class TeacherRoadmapDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Grade { get; set; }
        public string Subject { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<RoadmapModuleDto> Modules { get; set; } = new();
    }

    public class RoadmapModuleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
