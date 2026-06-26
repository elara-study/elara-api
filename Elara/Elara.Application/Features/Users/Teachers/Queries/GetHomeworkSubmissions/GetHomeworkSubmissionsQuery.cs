using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkSubmissions
{
    public class GetHomeworkSubmissionsQuery : IRequest<List<HomeworkSubmissionDto>>
    {
        public int ProblemSetId { get; set; }
        public string Status { get; set; } = "unrated"; // "unrated" or "rated"
    }
}
