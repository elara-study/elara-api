using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.Get_Class_Info
{
    public class GetClassInfoQueryHandler : IRequestHandler<GetClassInfoQuery, GetClassInfoResponse>
    {
        private readonly IClassRepository _classRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetClassInfoQueryHandler(IClassRepository classRepository, IMapper mapper, ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GetClassInfoResponse> Handle(GetClassInfoQuery request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var classEntity = await _classRepository.GetClassWithSubjectAsync(request.ClassId, cancellationToken);

            if (classEntity == null)
                throw new KeyNotFoundException($"Class with ID {request.ClassId} not found.");

            if (classEntity.TeacherId != teacherId)
                throw new UnauthorizedAccessException("You do not have access to this class.");

            return _mapper.Map<GetClassInfoResponse>(classEntity);
        }
    }
}
