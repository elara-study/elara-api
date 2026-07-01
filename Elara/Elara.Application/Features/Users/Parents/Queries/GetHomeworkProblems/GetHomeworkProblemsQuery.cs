using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetHomeworkProblems
{
    public class GetHomeworkProblemsQuery : IRequest<List<ParentHomeworkProblemDto>>
    {
        public int HomeworkId { get; set; }

        public GetHomeworkProblemsQuery(int homeworkId)
        {
            HomeworkId = homeworkId;
        }
    }
}
