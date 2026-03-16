namespace Elara.Application.Features.Users.Teachers.Commands.Create_Roadmap
{
    public class CreateRoadmapRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Grade { get; set; }
        public int SubjectId { get; set; }
    }
}
