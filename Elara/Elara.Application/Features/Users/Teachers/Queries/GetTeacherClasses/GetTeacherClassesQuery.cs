using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses
{
    public class GetTeacherClassesQuery : IRequest<List<GetTeacherClassesResponse>>
    {
        public Guid TeacherId { get; set; }
    }
}
