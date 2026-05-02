using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.AskForHint
{
    public class AskForHintCommandHandler : IRequestHandler<AskForHintCommand, HintDto>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IAsyncRepository<Hint, int> _hintRepository;
        private readonly IGeminiService _geminiService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AskForHintCommandHandler(
            IQuizRepository quizRepository, 
            IAsyncRepository<Hint, int> hintRepository,
            IGeminiService geminiService,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _quizRepository = quizRepository;
            _hintRepository = hintRepository;
            _geminiService = geminiService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<HintDto> Handle(AskForHintCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new Exception("User must be authenticated");
            
            var question = await _quizRepository.GetQuestionWithDetailsAsync(request.QuestionId, cancellationToken);
            if (question == null) throw new Exception("Question not found");

            var hint = question.Hints
                .OrderByDescending(h => h.StudentId == userId) 
                .ThenBy(h => h.StudentId == null)             
                .FirstOrDefault();
      
            if (hint == null)
            {
                // Generate AI Hint using question details
                var optionsText = string.Join(", ", question.Options.Select(o => o.Text));
                var prompt = $@"Provide a subtle educational hint for the following question. 
                             Do NOT give the answer. 
                             Question: {question.Text}
                             Options: {optionsText}
                             Requirement: The hint should be helpful but not direct. Keep it short.";

                var aiHintContent = await _geminiService.GenerateResponseAsync(prompt, "", [], cancellationToken);
                
                hint = new Hint
                {
                    Content = aiHintContent.Trim(),
                    QuestionId = question.Id,
                    StudentId = userId, 
                    IsAIGenerated = true,
                    HintLevel = 1 
                };

                await _hintRepository.AddAsync(hint, cancellationToken);
            }

            var session = await _quizRepository.GetSessionWithAnswersAsync(request.SessionId, cancellationToken);
            if (session == null) throw new Exception("Session not found");

            var answer = session.Answers.FirstOrDefault(a => a.QuestionId == request.QuestionId);
            if (answer == null)
            {
                session.Answers.Add(new QuizAnswer
                {
                    QuizSessionId = request.SessionId,
                    QuestionId = request.QuestionId,
                    QuestionType = question.QuestionType,
                    HintUsed = true,
                    IsCorrect = null
                });
            }
            else
            {
                answer.HintUsed = true;
            }

            await _quizRepository.UpdateAsync(session, cancellationToken);

            return _mapper.Map<HintDto>(hint);
        }
    }
}
