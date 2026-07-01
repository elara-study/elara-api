using Asp.Versioning;
using Elara.Application.Common;
using Elara.Application.Features.ChatAnalysisReport.Queries.GetParentChildInsights;
using Elara.API.Controllers.Requests;
using Elara.Application.Features.ChatAnalysisReport.Queries.GetSingleChildInsights;
using Elara.Application.Features.Notifications.Commands.MarkAllAsRead;
using Elara.Application.Features.Users.Parents.Queries.GetParentNotifications;
using Elara.Application.Features.Users.Parents.Queries.GetParentDashboard;
using Elara.Application.Features.Users.Parents.Queries.GetParentChildren;
using Elara.Application.Features.Users.Parents.Queries.GetParentProfile;
using Elara.Application.Features.Users.Parents.Queries.GetChildProfile;
using Elara.Application.Features.Users.Parents.Queries.GetChildHomework;
using Elara.Application.Features.Users.Parents.Queries.GetHomeworkProblems;
using Elara.Application.Features.Users.Parents.Queries.GetChildHomeworkSubmission;
using Elara.Application.Features.Users.Parents.Commands.LinkChild;
using Elara.Application.Features.Users.Parents.Commands.RespondLinkRequest;
using Elara.Application.Features.Users.Parents.Commands.RemoveChild;
using Elara.Application.Responses;
using Elara.Application.Models;
using Elara.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(BaseResponse<ParentDashboardDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetParentDashboard(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetParentDashboardQuery(), cancellationToken);
            return Ok(new BaseResponse<ParentDashboardDto>
            {
                Message = "Parent dashboard retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("children")]
        [ProducesResponseType(typeof(BaseResponse<ParentChildrenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetParentChildren(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetParentChildrenQuery(), cancellationToken);
            return Ok(new BaseResponse<ParentChildrenDto>
            {
                Message = "Children profiles and requests retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("children/link")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LinkChild([FromBody] LinkChildCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<bool>
            {
                Message = "Link request sent successfully.",
                Data = result
            });
        }

        [HttpPut("children/requests/{requestId:int}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RespondLinkRequest(int requestId, [FromBody] RespondLinkRequestRequest request, CancellationToken cancellationToken)
        {
            var command = new RespondLinkRequestCommand(requestId, request.action);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse<bool>
            {
                Message = "Request processed successfully.",
                Data = result
            });
        }

        [HttpGet("profile")]
        [ProducesResponseType(typeof(BaseResponse<ParentProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetParentProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetParentProfileQuery(), cancellationToken);
            return Ok(new BaseResponse<ParentProfileDto>
            {
                Message = "Profile retrieved successfully.",
                Data = result
            });
        }

        [HttpDelete("children/{childId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveChild(Guid childId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveChildCommand(childId), cancellationToken);
            return Ok(new BaseResponse<bool>
            {
                Message = "Child removed successfully from your account.",
                Data = result
            });
        }

        [HttpGet("/api/v{version:apiVersion}/children/{childId:guid}/profile")]
        [ProducesResponseType(typeof(BaseResponse<ChildProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChildProfile(Guid childId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetChildProfileQuery(childId), cancellationToken);
            return Ok(new BaseResponse<ChildProfileDto>
            {
                Message = "Child profile retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("/api/v{version:apiVersion}/children/{childId:guid}/homework")]
        [ProducesResponseType(typeof(BaseResponse<ChildHomeworkDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChildHomework(Guid childId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetChildHomeworkQuery(childId), cancellationToken);
            return Ok(new BaseResponse<ChildHomeworkDto>
            {
                Message = "Homework list retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("/api/v{version:apiVersion}/homework/{homeworkId:int}/problems")]
        [ProducesResponseType(typeof(BaseResponse<List<ParentHomeworkProblemDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHomeworkProblems(int homeworkId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetHomeworkProblemsQuery(homeworkId), cancellationToken);
            return Ok(new BaseResponse<List<ParentHomeworkProblemDto>>
            {
                Message = "Homework problems retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("/api/v{version:apiVersion}/children/{childId:guid}/homework/{homeworkId:int}/submission")]
        [ProducesResponseType(typeof(BaseResponse<ChildHomeworkSubmissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChildHomeworkSubmission(Guid childId, int homeworkId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetChildHomeworkSubmissionQuery(childId, homeworkId), cancellationToken);
            return Ok(new BaseResponse<ChildHomeworkSubmissionDto>
            {
                Message = "Submission details retrieved successfully.",
                Data = result
            });
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

        [HttpGet("/api/v{version:apiVersion}/alerts")]
        [ProducesResponseType(typeof(BaseResponse<ParentNotificationsResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetParentNotifications([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            var userId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
            var result = await _mediator.Send(new GetParentNotificationsQuery(userId, pagination.Page, pagination.Limit), cancellationToken);
            return Ok(new BaseResponse<ParentNotificationsResponseDto>
            {
                Message = "Notifications retrieved successfully.",
                Data = result
            });
        }

        [HttpPut("/api/v{version:apiVersion}/alerts/mark-all-read")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAllParentNotificationsAsRead(CancellationToken cancellationToken)
        {
            var userId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
            await _mediator.Send(new MarkAllAsReadCommand(userId), cancellationToken);
            return Ok(new BaseResponse<object>
            {
                Message = "All notifications marked as read.",
                Data = new { }
            });
        }
    }
}
