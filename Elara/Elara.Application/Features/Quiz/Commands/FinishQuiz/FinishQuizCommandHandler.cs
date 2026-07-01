using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Enums;
using System.Text.Json;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.FinishQuiz
{
    public class FinishQuizCommandHandler : IRequestHandler<FinishQuizCommand, QuizResultDto>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizService _quizService;
        private readonly IMapper _mapper;
        private readonly IAchievementEvaluationService _achievementService;

        public FinishQuizCommandHandler(IQuizRepository quizRepository, IQuizService quizService, IMapper mapper, IAchievementEvaluationService achievementService)
        {
            _quizRepository = quizRepository;
            _quizService = quizService;
            _mapper = mapper;
            _achievementService = achievementService;
        }

        public async Task<QuizResultDto> Handle(FinishQuizCommand request, CancellationToken cancellationToken)
        {
            var session = await _quizRepository.GetSessionWithAnswersAsync(request.SessionId, cancellationToken);
            if (session == null) throw new Exception("Session not found");

            if (session.Status == QuizSessionStatus.Completed)
                throw new Exception("Quiz already completed");

            if (string.IsNullOrEmpty(session.QuestionsJson))
                throw new Exception("No questions found for this session");

            var quizData = JsonSerializer.Deserialize<AIQuizResult>(session.QuestionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var totalQuestions = quizData?.Questions.Count ?? 0;

            session.CompletedAt = DateTime.UtcNow;
            session.Status = QuizSessionStatus.Completed;
            session.CorrectAnswers = session.Answers.Count(a => a.IsCorrect == true);
            session.WrongAnswers = session.Answers.Count(a => a.IsCorrect == false);
            session.UnansweredCount = totalQuestions - session.Answers.Count;

            var quizTitle = quizData?.Title ?? "Quiz";
            session.ElaraInsight = await _quizService.GenerateQuizInsightAsync(session.QuestionsJson, session.Answers, quizTitle, cancellationToken);

            int xpEarned = await _quizService.CalculateTotalXpAsync(session, cancellationToken);
            session.XpEarned = xpEarned;

            await _quizRepository.UpdateAsync(session, cancellationToken);

            await _quizService.UpdateStudentProgressAsync(session.StudentId, xpEarned, cancellationToken);

            await _achievementService.EvaluateStudentAchievementsAsync(session.StudentId, cancellationToken);

            var updatedSession = await _quizRepository.GetSessionWithAnswersAsync(session.Id, cancellationToken);

            return _mapper.Map<QuizResultDto>(updatedSession);
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
