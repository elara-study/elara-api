using Elara.Application.Features.Quiz.DTOs;
using Elara.Application.Responses;
using MediatR;

namespace Elara.Application.Features.Quiz.Commands.SubmitAnswer
{
    public class SubmitQuizAnswerCommand : IRequest<BaseResponse<SubmitAnswerResponse>>
    {
        public int SessionId { get; set; }
        public SubmitAnswerRequest Answer { get; set; } = null!;
    }
}
