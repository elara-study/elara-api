using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap
{
    public class CreateRoadmapCommand : IRequest<CreateRoadmapResponse>
    {
        public CreateRoadmapCommand(string name, int grade)
        {
            Name = name;
            Grade = grade;
        }

        public string Name { get; }
        public int Grade { get; }
    }
}
