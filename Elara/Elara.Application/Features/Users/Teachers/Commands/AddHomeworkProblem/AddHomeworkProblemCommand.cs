using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddHomeworkProblem
{
    public class AddHomeworkProblemCommand : IRequest<HomeworkProblemDto>
    {
        public AddHomeworkProblemCommand(int assignmentId, string description)
        {
            AssignmentId = assignmentId;
            Description = description;
        }

        public int AssignmentId { get; }
        public string Description { get; }
    }
}
