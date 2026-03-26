using Elara.Domain.Enums;

namespace Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap
{
    public class CreateRoadmapRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Grade { get; set; }
        public Subject Subject { get; set; }
    }
}
