using AutoMapper;
using Elara.Application.Contracts.Persistence.Quiz;
using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Queries.GetQuestions
{
    public class GetQuizQuestionsQueryHandler : IRequestHandler<GetQuizQuestionsQuery, QuizQuestionsListDto>
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IMapper _mapper;

        public GetQuizQuestionsQueryHandler(IQuizRepository quizRepository, IMapper mapper)
        {
            _quizRepository = quizRepository;
            _mapper = mapper;
        }

        public async Task<QuizQuestionsListDto> Handle(GetQuizQuestionsQuery request, CancellationToken cancellationToken)
        {
            var questions = await _quizRepository.GetQuestionsWithOptionsAsync(request.AssignmentId, cancellationToken);

            var questionsDto = _mapper.Map<List<QuizQuestionDto>>(questions);

            // Add sequential numbering
            for (int i = 0; i < questionsDto.Count; i++)
            {
                questionsDto[i].QuestionNumber = i + 1;
            }

            return new QuizQuestionsListDto
            {
                AssignmentId = request.AssignmentId,
                Questions = questionsDto
            };
        }
    }
}
