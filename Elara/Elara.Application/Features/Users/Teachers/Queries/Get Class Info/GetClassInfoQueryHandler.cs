using AutoMapper;
using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.Get_Class_Info
{
    public class GetClassInfoQueryHandler : IRequestHandler<GetClassInfoQuery, GetClassInfoResponse>
    {
        private readonly IClassRepository _classRepository;
        private readonly IMapper _mapper;
        public GetClassInfoQueryHandler(IClassRepository classRepository, IMapper mapper)
        {
            _classRepository = classRepository;
            _mapper = mapper;
        }

        public async Task<GetClassInfoResponse> Handle(GetClassInfoQuery request, CancellationToken cancellationToken)
        {
            var classEntity = await _classRepository.GetClassWithSubjectAsync(request.ClassId, cancellationToken);

            if (classEntity == null)
            {
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found.");
            }
            var response = _mapper.Map<GetClassInfoResponse>(classEntity);
           return response;
        }
    }
}
