using Asp.Versioning;
using Elara.API.Controllers.Requests;
using Elara.Application.Common;
using Elara.Application.Features.Chat.Commands.DeleteConversation;
using Elara.Application.Features.Chat.Commands.SendMessage;
using Elara.Application.Features.Chat.Commands.StartConversation;
using Elara.Application.Features.Chat.Queries.GetConversationHistory;
using Elara.Application.Features.Chat.Queries.GetConversations;
using Elara.Application.Features.Chat.Queries.GetRelevantChunks;
using Elara.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/conversations")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("/api/v{version:apiVersion}/chat")]
        [ProducesResponseType(typeof(BaseResponse<StartConversationResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> StartConversation([FromBody] StartConversationCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<StartConversationResponse>
            {
                Message = "Conversation started successfully.",
                Data = result
            });
        }

        [HttpPost("{id:guid}/messages")]
        [ProducesResponseType(typeof(BaseResponse<SendMessageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SendMessageCommand
            {
                ConversationId = id,
                Message = request.Message
            }, cancellationToken);

            return Ok(new BaseResponse<SendMessageResponse>
            {
                Message = "Message sent successfully.",
                Data = result
            });
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<ConversationSummaryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetConversations([FromQuery] PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetConversationsQuery { Pagination = pagination }, cancellationToken);
            return Ok(new BaseResponse<List<ConversationSummaryDto>>
            {
                Message = "Conversations retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<ConversationHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConversationHistory(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetConversationHistoryQuery { ConversationId = id }, cancellationToken);
            return Ok(new BaseResponse<ConversationHistoryDto>
            {
                Message = "Conversation history retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("relevant-chunks")]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRelevantChunks([FromBody] GetRelevantChunksQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new BaseResponse<string>
            {
                Message = "Relevant chunks retrieved successfully.",
                Data = result
            });
        }
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<Unit>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteConversation(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteConversationCommand { Id = id }, cancellationToken);
            return Ok(new BaseResponse<Unit>
            {
                Message = "Conversation deleted successfully."
            });
        }
    }
}
