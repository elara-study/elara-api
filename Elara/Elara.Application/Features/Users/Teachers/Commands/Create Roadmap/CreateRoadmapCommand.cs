using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.Create_Roadmap
{
    public class CreateRoadmapCommand : IRequest<CreateRoadmapResponse>
    {
        public CreateRoadmapCommand(string name, int grade, int subjectId, Guid teacherId)
        {
            Name = name;
            Grade = grade;
            SubjectId = subjectId;
            TeacherId = teacherId;
        }

        public string Name { get; }
        public int Grade { get; }
        public int SubjectId { get; }
        public Guid TeacherId { get; }
    }
}
