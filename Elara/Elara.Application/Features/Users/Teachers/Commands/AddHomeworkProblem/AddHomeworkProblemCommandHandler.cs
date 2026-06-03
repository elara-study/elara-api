using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddHomeworkProblem
{
    public class AddHomeworkProblemCommandHandler : IRequestHandler<AddHomeworkProblemCommand, HomeworkProblemDto>
    {
        private readonly IAsyncRepository<Assignment, int> _assignmentRepository;
        private readonly IAsyncRepository<Question, int> _questionRepository;

        public AddHomeworkProblemCommandHandler(
            IAsyncRepository<Assignment, int> assignmentRepository,
            IAsyncRepository<Question, int> questionRepository)
        {
            _assignmentRepository = assignmentRepository;
            _questionRepository = questionRepository;
        }

        public async Task<HomeworkProblemDto> Handle(AddHomeworkProblemCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
            if (assignment == null || assignment.AssignmentType != AssignmentType.Homework)
            {
                throw new KeyNotFoundException($"Homework assignment with id {request.AssignmentId} not found.");
            }

            var question = new Question
            {
                Text = request.Description,
                QuestionType = QuestionType.Essay,
                AssignmentId = request.AssignmentId,
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
