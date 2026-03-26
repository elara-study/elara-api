using Asp.Versioning;
using Elara.Application.Exceptions;
using Elara.Application.Features.Users.Teachers.Commands.CreateClass;
using Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap;
using Elara.Application.Features.Users.Teachers.Queries.GetClassInfo;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses;
using Elara.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Elara.Application.Responses;
using System.Security.Claims;

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
            try
            {
                var query = new GetTeacherClassesQuery();
                var result = await _mediator.Send(query);
                return Ok(new BaseResponse<List<GetTeacherClassesResponse>>
                {
                    Message = "Teacher classes retrieved successfully.",
                    Data = result
                });
            }
            catch (ValidationException ex)
            {
                return ValidationProblem(new ValidationProblemDetails(ex.Errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpPost("classes")]
        [ProducesResponseType(typeof(BaseResponse<CreateClassResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request)
        {
            try
            {
                var command = new CreateClassCommand(request.Name, request.Grade, request.RoadmapName);
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetClassInfo), new { id = result.Id }, new BaseResponse<CreateClassResponse>
                {
                    Message = "Class created successfully.",
                    Data = result
                });
            }
            catch (ValidationException ex)
            {
                return ValidationProblem(new ValidationProblemDetails(ex.Errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("roadmaps")]
        [ProducesResponseType(typeof(BaseResponse<CreateRoadmapResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateRoadmap([FromBody] CreateRoadmapRequest request)
        {
            try
            {
                var command = new CreateRoadmapCommand(request.Name, request.Grade, request.Subject);
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(CreateRoadmap), new BaseResponse<CreateRoadmapResponse>
                {
                    Message = "Roadmap created successfully.",
                    Data = result
                });
            }
            catch (ValidationException ex)
            {
                return ValidationProblem(new ValidationProblemDetails(ex.Errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("classes/{id}/info")]
        [ProducesResponseType(typeof(BaseResponse<GetClassInfoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetClassInfo(int id)
        {
            try
            {
                var query = new GetClassInfoQuery { ClassId = id };
                var result = await _mediator.Send(query);
                return Ok(new BaseResponse<GetClassInfoResponse>
                {
                    Message = "Class information retrieved successfully.",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ValidationException ex)
            {
                return ValidationProblem(new ValidationProblemDetails(ex.Errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}