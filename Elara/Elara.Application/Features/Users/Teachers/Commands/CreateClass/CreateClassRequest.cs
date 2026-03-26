namespace Elara.Application.Features.Users.Teachers.Commands.CreateClass
{
    public class CreateClassRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Grade { get; set; }
        public string? RoadmapName { get; set; }
    }
}