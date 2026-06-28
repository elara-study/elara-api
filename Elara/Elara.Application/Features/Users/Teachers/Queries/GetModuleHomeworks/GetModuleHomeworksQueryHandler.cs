using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetModuleHomeworks
{
    public class GetModuleHomeworksQueryHandler : IRequestHandler<GetModuleHomeworksQuery, List<ModuleHomeworkDto>>
    {
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetModuleHomeworksQueryHandler(
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            ICurrentUserService currentUserService)
        {
            _moduleRepository = moduleRepository;
            _homeworkRepository = homeworkRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<ModuleHomeworkDto>> Handle(GetModuleHomeworksQuery request, CancellationToken cancellationToken)
        {
            _ = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
            if (module == null || module.RoadmapId != request.RoadmapId)
                throw new KeyNotFoundException("Module not found.");

            var homeworks = await _homeworkRepository.FindAsync(
                h => h.ModuleId == request.ModuleId, cancellationToken);

            return homeworks
                .Where(h => !h.IsDeleted)
                .Select(h => new ModuleHomeworkDto
                {
                    Id = h.Id,
                    Title = h.Title,
                    Content = h.Content,
                    EstimatedDurationMinutes = h.EstimatedDurationMinutes,
                    CreatedAt = h.CreatedAt
                }).ToList();
        }
    }
}
