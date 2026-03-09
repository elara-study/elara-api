namespace Elara.Application.Features.Administrative.Classes.Commands.Create_Class
{
    public class CreateClassRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? RoadmapName { get; set; }
    }
}