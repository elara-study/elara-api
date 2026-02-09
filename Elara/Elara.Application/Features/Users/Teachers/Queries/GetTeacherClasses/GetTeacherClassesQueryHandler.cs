using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses
{
    public class GetTeacherClassesQueryHandler : IRequestHandler<GetTeacherClassesQuery, List<GetTeacherClassesResponse>>
    {
        private readonly IClassRepository _classRepository;

        public GetTeacherClassesQueryHandler(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task<List<GetTeacherClassesResponse>> Handle(GetTeacherClassesQuery request, CancellationToken cancellationToken)
        {
            var classes = await _classRepository.GetClassesByTeacherIdAsync(request.TeacherId, cancellationToken);

            return classes;
            

           
        }
    }
}
