using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.StartQuiz
{
    public class StartQuizSessionCommand : IRequest<QuizSessionDto>
    {
        public int AssignmentId { get; set; }
    }
}
