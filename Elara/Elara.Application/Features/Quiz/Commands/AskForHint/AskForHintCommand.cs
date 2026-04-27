using Elara.Application.Features.Quiz.DTOs;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.AskForHint
{
    public class AskForHintCommand : IRequest<HintDto>
    {
        public int SessionId { get; set; }
        public int QuestionId { get; set; }
    }
}
