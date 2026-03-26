using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap
{
    public class CreateRoadmapCommand : IRequest<CreateRoadmapResponse>
    {
        public CreateRoadmapCommand(string name, int grade, Subject subject)
        {
            Name = name;
            Grade = grade;
            Subject = subject;
        }

        public string Name { get; }
        public int Grade { get; }
        public Subject Subject { get; }
    }
}
