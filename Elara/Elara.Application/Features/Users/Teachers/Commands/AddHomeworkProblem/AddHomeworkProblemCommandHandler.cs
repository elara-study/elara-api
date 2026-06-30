using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddHomeworkProblem
{
    public class AddHomeworkProblemCommandHandler : IRequestHandler<AddHomeworkProblemCommand, HomeworkProblemDto>
    {
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly IAsyncRepository<Problem, int> _problemRepository;

        public AddHomeworkProblemCommandHandler(
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            IAsyncRepository<Problem, int> problemRepository)
        {
            _moduleRepository = moduleRepository;
            _homeworkRepository = homeworkRepository;
            _problemRepository = problemRepository;
        }

        public async Task<HomeworkProblemDto> Handle(AddHomeworkProblemCommand request, CancellationToken cancellationToken)
        {
            var module = (await _moduleRepository.FindAsync(m => m.PublicId == request.ModuleId, cancellationToken)).FirstOrDefault();
            if (module == null)
            {
                throw new KeyNotFoundException($"Module not found.");
            }

            var homework = (await _homeworkRepository.FindAsync(
                h => h.ModuleId == module.Id, cancellationToken)).FirstOrDefault();

            if (homework == null)
            {
                homework = new Homework
                {
                    Title = $"{module.Title} Homework",
                    ModuleId = module.Id,
                    MaxScore = 100,
                    DifficultyLevel = DifficultyLevel.Medium,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    Problems = new List<Problem>()
                };
                homework = await _homeworkRepository.AddAsync(homework, cancellationToken);
            }

            var problem = new Problem
            {
                Text = request.Description,
                QuestionType = QuestionType.Essay,
                HomeworkId = homework.Id,
                DifficultyLevel = DifficultyLevel.Medium,
                Options = new List<ProblemOption>()
            };

            problem = await _problemRepository.AddAsync(problem, cancellationToken);

            var dto = new HomeworkProblemDto
            {
                ProblemId = problem.Id,
                Description = problem.Text
            };

            return dto;
        }
    }
}
