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

        public FinishQuizCommandHandler(
            IQuizRepository quizRepository,
            IQuizService quizService,
            IMapper mapper)
        {
            _quizRepository = quizRepository;
            _quizService = quizService;
            _mapper = mapper;
        }

        public async Task<QuizResultDto> Handle(FinishQuizCommand request, CancellationToken cancellationToken)
        {
            var session = await _quizRepository.GetSessionWithAnswersAsync(request.SessionId, cancellationToken);
            if (session == null) throw new Exception("Session not found");

            if (session.Status == QuizSessionStatus.Completed)
                return _mapper.Map<QuizResultDto>(session);

            session.CompletedAt = DateTime.UtcNow;
            session.Status = QuizSessionStatus.Completed;

            // calculate the correct answers, wrong answers, and unanswered count
            session.CorrectAnswers = session.Answers.Count(a => a.IsCorrect == true);
            session.WrongAnswers = session.Answers.Count(a => a.IsCorrect == false);
            session.UnansweredCount = session.Assignment.Questions.Count - session.Answers.Count;
            
            // calculate the total xp earned
            session.XpEarned = await _quizService.CalculateTotalXpAsync(session, cancellationToken);

            // generate the AI insight
            var insight = await _quizService.GenerateQuizInsightAsync(session, cancellationToken);
            session.ElaraInsight = insight.Message;
            session.WeakTopics = string.Join(",", insight.WeakTopics);
            session.InsightRecommendation = insight.Recommendation;

            await _quizRepository.UpdateAsync(session, cancellationToken);

            await _quizService.UpdateStudentProgressAsync(session.StudentId, session.XpEarned, cancellationToken);

            var result = _mapper.Map<QuizResultDto>(session);
            result.ElaraInsight = insight; 

            return result;
        }
    }
}
