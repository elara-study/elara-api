using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Domain.Entities.Educational;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.GenerateQuiz
{
    public class GenerateQuizCommandHandler : IRequestHandler<GenerateQuizCommand, GeneratedQuizDto>
    {
        private readonly IQuizService _quizService;
        private readonly IAsyncRepository<Assignment, int> _assignmentRepository;
        private readonly IMapper _mapper;

        public GenerateQuizCommandHandler(
            IQuizService quizService, 
            IAsyncRepository<Assignment, int> assignmentRepository,
            IMapper mapper)
        {
            _quizService = quizService;
            _assignmentRepository = assignmentRepository;
            _mapper = mapper;
        }

        public async Task<GeneratedQuizDto> Handle(GenerateQuizCommand request, CancellationToken cancellationToken)
        {
            var assignmentId = await _quizService.GenerateQuizAsync(
                request.LessonId, 
                request.QuestionCount, 
                request.Difficulty, 
                request.QuestionTypes,
                cancellationToken);

            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
            if (assignment == null) throw new Exception("Failed to generate quiz");

            return _mapper.Map<GeneratedQuizDto>(assignment);
        }
    }
}
