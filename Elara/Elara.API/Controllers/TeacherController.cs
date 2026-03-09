using Asp.Versioning;
using Elara.Application.Exceptions;
using Elara.Application.Features.Administrative.Classes.Commands.Create_Class;
using Elara.Application.Features.Users.Teachers.Queries.Get_Class_Info;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/teacher")]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeacherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid? GetTeacherIdFromToken()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }

        [HttpGet("classes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTeacherClasses()
        {
            var teacherId = GetTeacherIdFromToken();
            if (teacherId is null)
                return Unauthorized(new { error = "Invalid or missing token." });

            try
            {
                var query = new GetTeacherClassesQuery { TeacherId = teacherId.Value };
                var result = await _mediator.Send(query);
                return Ok(result);
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassCommand command)
        {
            var teacherId = GetTeacherIdFromToken();
            if (teacherId is null)
                return Unauthorized(new { error = "Invalid or missing token." });

            try
            {
                command.TeacherId = teacherId.Value;
                await _mediator.Send(command);
                return NoContent();
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetClassInfo(int id)
        {
            try
            {
                var query = new GetClassInfoQuery { ClassId = id };
                var result = await _mediator.Send(query);
                return Ok(result);
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