using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.CreateClass
{
    public class CreateClassCommand : IRequest<CreateClassResponse>
    {
        public CreateClassCommand(string name, int grade, string? roadmapName = null)
        {
            Name = name;
            Grade = grade;
            RoadmapName = roadmapName;
        }

        public string Name { get; }
        public int Grade { get; }
        public string? RoadmapName { get; }
    }
}
