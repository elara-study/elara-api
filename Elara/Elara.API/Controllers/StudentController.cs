using Asp.Versioning;
using Elara.Application.Features.Users.Students.Commands.JoinGroup;
using Elara.Application.Features.Users.Students.Queries.GetStudentGroupOverview;
using Elara.Application.Features.Users.Students.Queries.GetStudentGroups;
using Elara.Application.Features.ChatAnalysisReport.Queries.GetAllReports;
using Elara.Application.Features.ChatAnalysisReport.Queries.GetConversationReport;
using Elara.Application.Responses;
using Elara.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/student")]
    [Authorize(Roles = Roles.Student)]
    public class StudentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private Guid StudentId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var studentId)
            ? studentId
            : throw new UnauthorizedAccessException("User is not authenticated.");

        public StudentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("groups")]
        [ProducesResponseType(typeof(BaseResponse<GetStudentGroupsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetGroups()
        {
            var response = await _mediator.Send(new GetStudentGroupsQuery(StudentId));
            return Ok(new BaseResponse<GetStudentGroupsResponse>
            {
                Message = "Student groups retrieved successfully.",
                Data = response
            });
        }

        [HttpGet("groups/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<GetStudentGroupOverviewResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGroupOverview(Guid id)
        {
            var response = await _mediator.Send(new GetStudentGroupOverviewQuery(StudentId, id));
            return Ok(new BaseResponse<GetStudentGroupOverviewResponse>
            {
                Message = "Student group overview retrieved successfully.",
                Data = response
            });
        }

        [HttpPost("groups/join")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> JoinGroup([FromBody] JoinGroupRequest request)
        {
            await _mediator.Send(new JoinGroupCommand(StudentId, request.JoinCode));
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<object>
            {
                Message = "Joined group successfully.",
                Data = new { }
            });
        }

        [HttpGet("insights")]
        [ProducesResponseType(typeof(BaseResponse<List<GetReportsDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetInsights(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllReportsQuery(), cancellationToken);
            return Ok(new BaseResponse<List<GetReportsDto>>
            {
                Message = "Insights retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("insights/{conversationId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<ConversationReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConversationReport(Guid conversationId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetConversationReportQuery { ConversationId = conversationId }, cancellationToken);
            return Ok(new BaseResponse<ConversationReportDto>
            {
                Message = "Report retrieved successfully.",
                Data = result
            });
        }
    }
}
