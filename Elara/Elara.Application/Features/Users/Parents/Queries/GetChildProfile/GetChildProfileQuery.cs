using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetChildProfile
{
    public class GetChildProfileQuery : IRequest<ChildProfileDto>
    {
        public Guid ChildId { get; set; }

        public GetChildProfileQuery(Guid childId)
        {
            ChildId = childId;
        }
    }
}
