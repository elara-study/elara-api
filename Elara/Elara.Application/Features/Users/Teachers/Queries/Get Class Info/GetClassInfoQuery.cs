using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.Get_Class_Info
{
    public class GetClassInfoQuery : IRequest<GetClassInfoResponse>
    {
        public int ClassId { get; set; }
        public Guid TeacherId { get; set; }
    }
}
