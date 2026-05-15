using Asp.Versioning;
using Elara.Application.Features.ChatAnalysisReport.Queries.GetParentChildInsights;
using Elara.Application.Features.ChatAnalysisReport.Queries.GetSingleChildInsights;
using Elara.Application.Responses;
using Elara.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/parent")]
    [Authorize(Roles = Roles.Parent)]
    public class ParentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ParentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("child-insights")]
        [ProducesResponseType(typeof(BaseResponse<List<ParentChildInsightDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetChildInsights(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetParentChildInsightsQuery(), cancellationToken);
            return Ok(new BaseResponse<List<ParentChildInsightDto>>
            {
                Message = "Child insights retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("children/{childId:guid}/insights")]
        [ProducesResponseType(typeof(BaseResponse<SingleChildInsightDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSingleChildInsights(Guid childId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetSingleChildInsightsQuery { ChildId = childId }, cancellationToken);
            return Ok(new BaseResponse<SingleChildInsightDto>
            {
                Message = "Child insights retrieved successfully.",
                Data = result
            });
        }
    }
}
