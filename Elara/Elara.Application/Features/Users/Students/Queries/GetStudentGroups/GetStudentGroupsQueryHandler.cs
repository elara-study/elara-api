using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroups
{
    public class GetStudentGroupsQueryHandler : IRequestHandler<GetStudentGroupsQuery, GetStudentGroupsResponse>
    {
        private readonly IClassRepository _classRepository;

        public GetStudentGroupsQueryHandler(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task<GetStudentGroupsResponse> Handle(GetStudentGroupsQuery request, CancellationToken cancellationToken)
        {
            var groups = await _classRepository.GetStudentGroupsByStudentIdAsync(request.StudentId, cancellationToken);

            return new GetStudentGroupsResponse
            {
                Groups = groups
            };
        }
    }
}
