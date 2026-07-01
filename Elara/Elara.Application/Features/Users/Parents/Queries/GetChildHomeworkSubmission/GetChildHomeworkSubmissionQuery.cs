using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetChildHomeworkSubmission
{
    public class GetChildHomeworkSubmissionQuery : IRequest<ChildHomeworkSubmissionDto>
    {
        public Guid ChildId { get; set; }
        public int HomeworkId { get; set; }

        public GetChildHomeworkSubmissionQuery(Guid childId, int homeworkId)
        {
            ChildId = childId;
            HomeworkId = homeworkId;
        }
    }
}
