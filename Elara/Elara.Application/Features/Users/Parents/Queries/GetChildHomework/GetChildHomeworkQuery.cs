using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetChildHomework
{
    public class GetChildHomeworkQuery : IRequest<ChildHomeworkDto>
    {
        public Guid ChildId { get; set; }

        public GetChildHomeworkQuery(Guid childId)
        {
            ChildId = childId;
        }
    }
}
