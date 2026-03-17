using MediatR;

namespace Elara.Application.Features.Administrative.Classes.Commands.Create_Class
{
    public class CreateClassCommand : IRequest
    {
        public CreateClassCommand(string name, int grade, string subject, string? roadmapName = null)
        {
            Name = name;
            Grade = grade;
            Subject = subject;
            RoadmapName = roadmapName;
        }

        public string Name { get; }
        public int Grade { get; }
        public string Subject { get; }
        public string? RoadmapName { get; }
    }
}
