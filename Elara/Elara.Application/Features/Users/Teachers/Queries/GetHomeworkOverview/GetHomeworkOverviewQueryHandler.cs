using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview
{
    public class GetHomeworkOverviewQueryHandler : IRequestHandler<GetHomeworkOverviewQuery, HomeworkOverviewDto>
    {
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<ProblemSet, int> _problemSetRepository;
        private readonly IAsyncRepository<Question, int> _questionRepository;

        public GetHomeworkOverviewQueryHandler(
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<ProblemSet, int> problemSetRepository,
            IAsyncRepository<Question, int> questionRepository)
        {
            _moduleRepository = moduleRepository;
            _problemSetRepository = problemSetRepository;
            _questionRepository = questionRepository;
        }

        public async Task<HomeworkOverviewDto> Handle(GetHomeworkOverviewQuery request, CancellationToken cancellationToken)
        {
            var module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
            if (module == null)
            {
                throw new KeyNotFoundException($"Module with id {request.ModuleId} not found.");
            }

            var problemSet = (await _problemSetRepository.FindAsync(
                a => a.ModuleId == request.ModuleId && a.ProblemSetType == ProblemSetType.ProblemSet,
                cancellationToken)).FirstOrDefault();

            if (problemSet == null)
            {
                problemSet = new ProblemSet
                {
                    Title = $"{module.Title} Homework",
                    ModuleId = request.ModuleId,
                    ProblemSetType = ProblemSetType.ProblemSet,
                    MaxScore = 100,
                    DifficultyLevel = DifficultyLevel.Medium,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    Questions = new List<Question>()
                };
                problemSet = await _problemSetRepository.AddAsync(problemSet, cancellationToken);
            }

            var questions = await _questionRepository.FindAsync(
                q => q.ProblemSetId == problemSet.Id, cancellationToken);

            var dto = new HomeworkOverviewDto
            {
                ProblemSetId = problemSet.Id,
                ModuleName = module.Title,
                Overview = new HomeworkOverviewStats { TotalScore = problemSet.MaxScore }
            };

            foreach (var q in questions)
            {
                dto.Problems.Add(new HomeworkProblemDto
                {
                    Id = q.Id,
                    Question = q.Text,
                    Type = "text",
                    AllowImageUpload = true
                });
            }

            return dto;
        }
    }
}
