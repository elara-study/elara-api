using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using System.Text.Json;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.GenerateQuiz
{
    public class GenerateQuizCommandHandler : IRequestHandler<GenerateQuizCommand, GeneratedQuizDto>
    {
        private readonly IQuizService _quizService;
        private readonly IQuizRepository _quizRepository;
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IClassRepository _classRepository;

        public GenerateQuizCommandHandler(
            IQuizService quizService,
            IQuizRepository quizRepository,
            IAsyncRepository<Module, int> moduleRepository,
            ICurrentUserService currentUserService,
            IClassRepository classRepository)
        {
            _quizService = quizService;
            _quizRepository = quizRepository;
            _moduleRepository = moduleRepository;
            _currentUserService = currentUserService;
            _classRepository = classRepository;
        }

        public async Task<GeneratedQuizDto> Handle(GenerateQuizCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? throw new Exception("User not authenticated");

            var classEntity = await _classRepository.GetClassWithSubjectByPublicIdAsync(request.GroupId, cancellationToken);
            if (classEntity == null) throw new Exception("Group not found");

            var isEnrolled = await _classRepository.IsStudentJoinedClassAsync(userId, classEntity.Id, cancellationToken);
            if (!isEnrolled) throw new Exception("You are not enrolled in this group");

            var module = (await _moduleRepository.FindAsync(m => m.PublicId == request.ModuleId, cancellationToken)).FirstOrDefault();
            if (module == null) throw new Exception("Module not found");

            if (module.RoadmapId != classEntity.RoadmapId)
                throw new Exception("Module does not belong to this group's roadmap");

            var result = await _quizService.GenerateQuizAsync(
                module.Id,
                request.QuestionCount,
                request.DifficultyLevel,
                request.QuestionTypes,
                cancellationToken);

            var session = new QuizSession
            {
                StudentId = userId,
                ModuleId = module.Id,
                QuestionsJson = result.QuestionsJson,
                StartedAt = DateTime.UtcNow,
                Status = Domain.Enums.QuizSessionStatus.InProgress
            };

            session = await _quizRepository.AddAsync(session, cancellationToken);

            // Parse questions for response (strip correct answers)
            var quizData = JsonSerializer.Deserialize<AIQuizResult>(result.QuestionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var questions = quizData?.Questions.Select((q, i) => new QuizQuestionDto
            {
                QuestionNumber = i + 1,
                Text = q.Text,
                QuestionType = q.Type,
                HasHint = !string.IsNullOrEmpty(q.Hint),
                Options = q.Options?.Select(o => o.Text).ToList() ?? new()
            }).ToList() ?? new();

            return new GeneratedQuizDto
            {
                SessionId = session.Id,
                Title = quizData?.Title ?? $"{module.Title} Quiz",
                TotalQuestions = questions.Count,
                ModuleName = module.Title,
                SubjectName = module.Subject?.Name ?? "",
                Questions = questions
            };
        }

        private class AIQuizResult
        {
            public string Title { get; set; } = string.Empty;
            public List<AIQuestion> Questions { get; set; } = new();
        }

        private class AIQuestion
        {
            public string Text { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Hint { get; set; } = string.Empty;
            public List<AIOption>? Options { get; set; }
        }

        private class AIOption
        {
            public string Text { get; set; } = string.Empty;
            public bool IsCorrect { get; set; }
        }
    }
}
