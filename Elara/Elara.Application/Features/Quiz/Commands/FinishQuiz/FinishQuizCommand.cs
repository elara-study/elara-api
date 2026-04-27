using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.FinishQuiz
{
    public class FinishQuizCommand : IRequest<QuizResultDto>
    {
        public int SessionId { get; set; }
    }
}
