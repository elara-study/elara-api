using AutoMapper;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Educational;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.AskForHint
{
    public class AskForHintCommandHandler : IRequestHandler<AskForHintCommand, HintDto>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IAsyncRepository<Question, int> _questionRepository;
        private readonly IMapper _mapper;

        public AskForHintCommandHandler(
            IQuizRepository quizRepository, 
            IAsyncRepository<Question, int> questionRepository,
            IMapper mapper)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        public async Task<HintDto> Handle(AskForHintCommand request, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
            if (question == null) throw new Exception("Question not found");

            var hint = question.Hints.FirstOrDefault();
            if (hint == null) throw new Exception("No hints available for this question");

            var answer = await _quizRepository.GetAnswerAsync(request.SessionId, request.QuestionId, cancellationToken);
            if (answer != null)
            {
                answer.HintUsed = true;
            }

            return _mapper.Map<HintDto>(hint);
        }
    }
}
