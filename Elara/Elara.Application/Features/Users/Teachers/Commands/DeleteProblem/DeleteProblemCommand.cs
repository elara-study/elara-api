using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.DeleteProblem
{
    public class DeleteProblemCommand : IRequest
    {
        public int ProblemId { get; }

        public DeleteProblemCommand(int problemId)
        {
            ProblemId = problemId;
        }
    }
}
