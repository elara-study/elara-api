using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Queries.GetQuestions
{
    public class GetQuizQuestionsQuery : IRequest<QuizQuestionsListDto>
    {
        public int AssignmentId { get; set; }
    }
}
