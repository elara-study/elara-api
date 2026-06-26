using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroupModules
{
    public class GetStudentGroupModulesHandler : IRequestHandler<GetStudentGroupModulesQuery, GetStudentGroupModulesResponse>
    {
        private readonly IClassRepository _classRepository;

        public GetStudentGroupModulesHandler(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task<GetStudentGroupModulesResponse> Handle(GetStudentGroupModulesQuery request, CancellationToken cancellationToken)
        {
            var response = await _classRepository.GetStudentGroupModulesAsync(request.StudentId, request.GroupId, cancellationToken);

            if (response == null)
            {
                throw new KeyNotFoundException("Group not found or you don't have access.");
            }

            return response;
        }
    }
}
