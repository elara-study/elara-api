using Asp.Versioning;
using Elara.API.Controllers.Requests;
using Elara.Application.Common;
using Elara.Application.Features.Notifications;
using Elara.Application.Features.Notifications.Commands.MarkAllAsRead;
using Elara.Application.Features.Notifications.Commands.MarkAsRead;
using Elara.Application.Features.Notifications.Commands.MarkBatchAsRead;
using Elara.Application.Features.Notifications.Commands.RegisterDeviceToken;
using Elara.Application.Features.Notifications.Commands.RemoveDeviceToken;
using Elara.Application.Features.Notifications.Commands.UpdatePreferences;
using Elara.Application.Features.Notifications.Queries.GetNotifications;
using Elara.Application.Features.Notifications.Queries.GetPreferences;
using Elara.Application.Features.Notifications.Queries.GetUnreadCount;
using Elara.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private Guid UserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException("User is not authenticated.");

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<NotificationDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetNotifications([FromQuery] PaginationParams pagination)
        {
            var result = await _mediator.Send(new GetNotificationsQuery(UserId, pagination.Page, pagination.Limit));
            return Ok(new BaseResponse<List<NotificationDto>>
            {
                Message = "Notifications retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(BaseResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUnreadCount()
        {
            var result = await _mediator.Send(new GetUnreadCountQuery(UserId));
            return Ok(new BaseResponse<int>
            {
                Message = "Unread count retrieved successfully.",
                Data = result
            });
        }

        [HttpPatch("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _mediator.Send(new MarkAsReadCommand(id, UserId));
            return Ok(new BaseResponse<object>
            {
                Message = "Notification marked as read.",
                Data = new { }
            });
        }

        [HttpPatch("batch")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkBatchAsRead([FromBody] MarkBatchAsReadRequest request)
        {
            await _mediator.Send(new MarkBatchAsReadCommand(request.NotificationIds, UserId));
            return Ok(new BaseResponse<object>
            {
                Message = "Notifications marked as read.",
                Data = new { }
            });
        }

        [HttpPatch]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _mediator.Send(new MarkAllAsReadCommand(UserId));
            return Ok(new BaseResponse<object>
            {
                Message = "All notifications marked as read.",
                Data = new { }
            });
        }

        [HttpPost("device-token")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterDeviceToken([FromBody] RegisterDeviceTokenRequest request)
        {
            await _mediator.Send(new RegisterDeviceTokenCommand(UserId, request.Token));
            return Ok(new BaseResponse<object>
            {
                Message = "Device token registered successfully.",
                Data = new { }
            });
        }

        [HttpDelete("device-token")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveDeviceToken([FromBody] RegisterDeviceTokenRequest request)
        {
            await _mediator.Send(new RemoveDeviceTokenCommand(request.Token));
            return Ok(new BaseResponse<object>
            {
                Message = "Device token removed successfully.",
                Data = new { }
            });
        }

        [HttpGet("preferences")]
        [ProducesResponseType(typeof(BaseResponse<PreferencesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPreferences()
        {
            var result = await _mediator.Send(new GetPreferencesQuery(UserId));
            return Ok(new BaseResponse<PreferencesDto>
            {
                Message = "Preferences retrieved successfully.",
                Data = result
            });
        }

        [HttpPatch("preferences")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request)
        {
            await _mediator.Send(new UpdatePreferencesCommand(
                UserId,
                request.GroupUpdates,
                request.StreakReminders,
                request.HomeworkReminders,
                request.NewLessons,
                request.AiProgressReports));
            return Ok(new BaseResponse<object>
            {
                Message = "Preferences updated successfully.",
                Data = new { }
            });
        }
    }
}
