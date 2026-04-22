using System;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetGroupStudents
{
    public class GetGroupStudentsQuery : IRequest<List<GetGroupStudentsResponse>>
    {
        public Guid ClassId { get; set; }
    }
}
