using Asp.Versioning;
using Elara.Application.Features.Quiz.DTOs;
using Elara.Application.Features.Quiz.Commands.FinishQuiz;
using Elara.Application.Features.Quiz.Commands.GenerateQuiz;
using Elara.Application.Features.Quiz.Commands.SubmitAnswer;
using Elara.Application.Features.Quiz.Queries.GetHistory;
using Elara.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuizController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("generate")]
        [ProducesResponseType(typeof(BaseResponse<GeneratedQuizDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> GenerateQuiz([FromBody] GenerateQuizCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<GeneratedQuizDto>
            {
                Message = "Quiz generated successfully.",
                Data = result
            });
        }

        [HttpPost("sessions/{sessionId}/answers")]
        [ProducesResponseType(typeof(BaseResponse<SubmitAnswerResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SubmitAnswer(int sessionId, [FromBody] SubmitAnswerRequest request)
        {
            var result = await _mediator.Send(new SubmitQuizAnswerCommand { SessionId = sessionId, Answer = request });
            return Ok(result);
        }

        [HttpPost("sessions/{sessionId}/complete")]
        [ProducesResponseType(typeof(BaseResponse<QuizResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CompleteQuiz(int sessionId)
        {
            var result = await _mediator.Send(new FinishQuizCommand { SessionId = sessionId });
            return Ok(new BaseResponse<QuizResultDto>
            {
                Message = "Quiz completed successfully.",
                Data = result
            });
        }

        [HttpGet("history")]
        [ProducesResponseType(typeof(BaseResponse<QuizHistoryListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistory([FromQuery] GetQuizHistoryQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<QuizHistoryListDto>
            {
                Message = "Quiz history retrieved successfully.",
                Data = result
            });
        }
    }
}
