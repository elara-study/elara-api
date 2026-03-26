using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetClassInfo
{
    public class GetClassInfoQuery : IRequest<GetClassInfoResponse>
    {
        public int ClassId { get; set; }
    }
}
