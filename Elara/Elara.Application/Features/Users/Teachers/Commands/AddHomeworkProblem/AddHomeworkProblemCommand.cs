using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddHomeworkProblem
{
    public class AddHomeworkProblemCommand : IRequest<HomeworkProblemDto>
    {
        public AddHomeworkProblemCommand(int problemSetId, string description)
        {
            ProblemSetId = problemSetId;
            Description = description;
        }

        public int ProblemSetId { get; }
        public string Description { get; }
    }
}
