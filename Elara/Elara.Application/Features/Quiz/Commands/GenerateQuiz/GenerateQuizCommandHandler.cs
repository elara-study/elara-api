using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.GenerateQuiz
{
    public class GenerateQuizCommandHandler : IRequestHandler<GenerateQuizCommand, GeneratedQuizDto>
    {
        private readonly IQuizService _quizService;
        private readonly IQuizRepository _quizRepository;
        private readonly IMapper _mapper;

        public GenerateQuizCommandHandler(
            IQuizService quizService, 
            IQuizRepository quizRepository,
            IMapper mapper)
        {
            _quizService = quizService;
            _quizRepository = quizRepository;
            _mapper = mapper;
        }

        public async Task<GeneratedQuizDto> Handle(GenerateQuizCommand request, CancellationToken cancellationToken)
        {
            var assignmentId = await _quizService.GenerateQuizAsync(
                request.LessonId, 
                request.QuestionCount, 
                request.DifficultyLevel, 
                request.QuestionTypes,
                cancellationToken);

            var assignment = await _quizRepository.GetAssignmentWithDetailsAsync(assignmentId, cancellationToken);
            
            return _mapper.Map<GeneratedQuizDto>(assignment);
        }
    }
}
