using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;


namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview
{
    public class GetHomeworkOverviewQueryHandler : IRequestHandler<GetHomeworkOverviewQuery, HomeworkOverviewDto>
    {
        private readonly IAsyncRepository<Topic, int> _topicRepository;
        private readonly IAsyncRepository<Assignment, int> _assignmentRepository;
        private readonly IAsyncRepository<Question, int> _questionRepository;

        public GetHomeworkOverviewQueryHandler(
            IAsyncRepository<Topic, int> topicRepository,
            IAsyncRepository<Assignment, int> assignmentRepository,
            IAsyncRepository<Question, int> questionRepository)
        {
            _topicRepository = topicRepository;
            _assignmentRepository = assignmentRepository;
            _questionRepository = questionRepository;
        }

        public async Task<HomeworkOverviewDto> Handle(GetHomeworkOverviewQuery request, CancellationToken cancellationToken)
        {
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
            if (topic == null)
            {
                throw new KeyNotFoundException($"Topic with id {request.TopicId} not found.");
            }

            var assignment = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                _assignmentRepository.AsQueryable(),
                a => a.TopicId == request.TopicId && a.AssignmentType == AssignmentType.Homework,
                cancellationToken);
            if (assignment == null)
            {
                assignment = new Assignment
                {
                    Title = $"{topic.Title} Homework",
                    TopicId = request.TopicId,
                    AssignmentType = AssignmentType.Homework,
                    MaxScore = 100,
                    DifficultyLevel = DifficultyLevel.Medium,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    Questions = new List<Question>()
                };
                assignment = await _assignmentRepository.AddAsync(assignment, cancellationToken);
            }

            var questions = _questionRepository.AsQueryable()
                .Where(q => q.AssignmentId == assignment.Id)
                .ToList();

            var dto = new HomeworkOverviewDto
            {
                AssignmentId = assignment.Id,
                Topic = topic.Title,
                Overview = new HomeworkOverviewStats { TotalScore = assignment.MaxScore }
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
