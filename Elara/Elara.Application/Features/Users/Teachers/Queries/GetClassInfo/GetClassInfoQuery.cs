using MediatR;
using System;

namespace Elara.Application.Features.Users.Teachers.Queries.GetClassInfo
{
    public class GetClassInfoQuery : IRequest<GetClassInfoResponse>
    {
        public Guid ClassId { get; set; }
    }
}
