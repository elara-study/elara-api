using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.FinishQuiz
{
    public class FinishQuizCommandHandler : IRequestHandler<FinishQuizCommand, QuizResultDto>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizService _quizService;
        private readonly IMapper _mapper;

        public FinishQuizCommandHandler(IQuizRepository quizRepository, IQuizService quizService, IMapper mapper)
        {
            _quizRepository = quizRepository;
            _quizService = quizService;
            _mapper = mapper;
        }

        public async Task<QuizResultDto> Handle(FinishQuizCommand request, CancellationToken cancellationToken)
        {
            // Get session with all details
            var session = await _quizRepository.GetSessionWithAnswersAsync(request.SessionId, cancellationToken);
            if (session == null) throw new Exception("Session not found");

            if (session.Status == QuizSessionStatus.Completed)
                throw new Exception("Quiz already completed");

            // Finalize session stats
            session.CompletedAt = DateTime.UtcNow;
            session.Status = QuizSessionStatus.Completed;
            session.CorrectAnswers = session.Answers.Count(a => a.IsCorrect == true);
            session.WrongAnswers = session.Answers.Count(a => a.IsCorrect != true);
            session.UnansweredCount = session.Assignment.Questions.Count - session.Answers.Count;
            
            // Generate AI Insights and XP
            var insight = await _quizService.GenerateQuizInsightAsync(session, cancellationToken);
            session.ElaraInsight = insight.Message;
            session.InsightRecommendation = insight.Recommendation;
            session.WeakTopics = string.Join(",", insight.WeakTopics);
            
            int xpEarned = await _quizService.CalculateTotalXpAsync(session, cancellationToken);
            session.XpEarned = xpEarned;

            await _quizRepository.UpdateAsync(session, cancellationToken);

            await _quizService.UpdateStudentProgressAsync(session.StudentId, xpEarned, cancellationToken);

            var updatedSession = await _quizRepository.GetSessionWithAnswersAsync(session.Id, cancellationToken);

            return _mapper.Map<QuizResultDto>(updatedSession);
        }
    }
}
