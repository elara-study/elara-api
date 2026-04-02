using Asp.Versioning;
using Elara.Application.Features.Users.Teachers.Commands.CreateClass;
using Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap;
using Elara.Application.Features.Users.Teachers.Commands.AddAnnouncement;
using Elara.Application.Features.Users.Teachers.Commands.DeleteAnnouncement;
using Elara.Application.Features.Users.Teachers.Queries.GetAnnouncements;
using Elara.Application.Features.Users.Teachers.Queries.GetClassInfo;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses;
using Elara.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Elara.Application.Responses;
using System;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/teacher")]
    [Authorize(Roles = Roles.Teacher)]
    public class TeacherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeacherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("classes")]
        [ProducesResponseType(typeof(BaseResponse<List<GetTeacherClassesResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetTeacherClasses()
        {
            var query = new GetTeacherClassesQuery();
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<List<GetTeacherClassesResponse>>
            {
                Message = "Teacher classes retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("classes")]
        [ProducesResponseType(typeof(BaseResponse<CreateClassResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request)
        {
            var command = new CreateClassCommand(request.Name, request.Grade, request.RoadmapName);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetClassInfo), new { id = result.Id }, new BaseResponse<CreateClassResponse>
            {
                Message = "Class created successfully.",
                Data = result
            });
        }

        [HttpPost("roadmaps")]
        [ProducesResponseType(typeof(BaseResponse<CreateRoadmapResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateRoadmap([FromBody] CreateRoadmapRequest request)
        {
            var command = new CreateRoadmapCommand(request.Name, request.Grade, request.Subject);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateRoadmap), new BaseResponse<CreateRoadmapResponse>
            {
                Message = "Roadmap created successfully.",
                Data = result
            });
        }

        [HttpGet("classes/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<GetClassInfoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetClassInfo(Guid id)
        {
            var query = new GetClassInfoQuery { ClassId = id };
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<GetClassInfoResponse>
            {
                Message = "Class information retrieved successfully.",
                Data = result
            });
        }

        [HttpPost("groups/{id:guid}/announcements")]
        [ProducesResponseType(typeof(BaseResponse<AddAnnouncementResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddAnnouncement(Guid id, [FromBody] AddAnnouncementRequest request)
        {
            var command = new AddAnnouncementCommand(id, request.Title, request.Content);
            var result = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse<AddAnnouncementResponse>
            {
                Message = "Announcement added successfully.",
                Data = result
            });
        }

        [HttpGet("groups/{id:guid}/announcements")]
        [ProducesResponseType(typeof(BaseResponse<List<GetAnnouncementsResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAnnouncements(Guid id)
        {
            var query = new GetAnnouncementsQuery { ClassId = id };
            var result = await _mediator.Send(query);
            return Ok(new BaseResponse<List<GetAnnouncementsResponse>>
            {
                Message = "Announcements retrieved successfully.",
                Data = result
            });
        }

        [HttpDelete("groups/{id:guid}/announcements/{announcementId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAnnouncement(Guid id, Guid announcementId)
        {
            var command = new DeleteAnnouncementCommand(id, announcementId);
            await _mediator.Send(command);
            return Ok(new BaseResponse<object>
            {
                Message = "Announcement deleted successfully.",
                Data = null
            });
        }
    }
}