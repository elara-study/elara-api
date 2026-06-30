using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview
{
    public class GetHomeworkOverviewQueryHandler : IRequestHandler<GetHomeworkOverviewQuery, HomeworkOverviewDto>
    {
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly IAsyncRepository<Problem, int> _problemRepository;

        public GetHomeworkOverviewQueryHandler(
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            IAsyncRepository<Problem, int> problemRepository)
        {
            _moduleRepository = moduleRepository;
            _homeworkRepository = homeworkRepository;
            _problemRepository = problemRepository;
        }

        public async Task<HomeworkOverviewDto> Handle(GetHomeworkOverviewQuery request, CancellationToken cancellationToken)
        {
            var module = (await _moduleRepository.FindAsync(m => m.PublicId == request.ModuleId, cancellationToken)).FirstOrDefault();
            if (module == null)
            {
                throw new KeyNotFoundException($"Module not found.");
            }

            var homework = await _homeworkRepository.FindAsync(
                h => h.ModuleId == module.Id, cancellationToken);

            var foundHomework = homework.FirstOrDefault();

            var dto = new HomeworkOverviewDto
            {
                ModuleName = module.Title,
                TotalScoreXp = foundHomework?.MaxScore ?? 0
            };

            if (foundHomework != null)
            {
                var problems = await _problemRepository.FindAsync(
                    p => p.HomeworkId == foundHomework.Id, cancellationToken);

                foreach (var p in problems)
                {
                    dto.Problems.Add(new HomeworkProblemDto
                    {
                        ProblemId = p.Id,
                        Description = p.Text
                    });
                }
            }

            return dto;
        }
    }
}
