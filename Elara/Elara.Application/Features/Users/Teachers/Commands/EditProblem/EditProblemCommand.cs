using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.EditProblem
{
    public class EditProblemCommand : IRequest<HomeworkProblemDto>
    {
        public int ProblemId { get; }
        public string Text { get; }

        public EditProblemCommand(int problemId, string text)
        {
            ProblemId = problemId;
            Text = text;
        }
    }
}
