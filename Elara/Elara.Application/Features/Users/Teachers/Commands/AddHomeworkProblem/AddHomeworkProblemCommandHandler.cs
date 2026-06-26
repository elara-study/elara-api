using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddHomeworkProblem
{
    public class AddHomeworkProblemCommandHandler : IRequestHandler<AddHomeworkProblemCommand, HomeworkProblemDto>
    {
        private readonly IAsyncRepository<ProblemSet, int> _problemSetRepository;
        private readonly IAsyncRepository<Question, int> _questionRepository;

        public AddHomeworkProblemCommandHandler(
            IAsyncRepository<ProblemSet, int> problemSetRepository,
            IAsyncRepository<Question, int> questionRepository)
        {
            _problemSetRepository = problemSetRepository;
            _questionRepository = questionRepository;
        }

        public async Task<HomeworkProblemDto> Handle(AddHomeworkProblemCommand request, CancellationToken cancellationToken)
        {
            var problemSet = await _problemSetRepository.GetByIdAsync(request.ProblemSetId, cancellationToken);
            if (problemSet == null || problemSet.ProblemSetType != ProblemSetType.ProblemSet)
            {
                throw new KeyNotFoundException($"Homework assignment with id {request.ProblemSetId} not found.");
            }

            var question = new Question
            {
                Text = request.Description,
                QuestionType = QuestionType.Essay,
                ProblemSetId = request.ProblemSetId,
                DifficultyLevel = DifficultyLevel.Medium,
                Options = new List<QuestionOption>()
            };

            question = await _questionRepository.AddAsync(question, cancellationToken);

            var dto = new HomeworkProblemDto
            {
                Id = question.Id,
                Question = question.Text,
                Type = "text",
                AllowImageUpload = true
            };

            return dto;
        }
    }
}
