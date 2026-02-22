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
    public class TeacherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeacherController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("classes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTeacherClasses()
        {
            try
            { 
                var teacherIdHeader = Request.Headers["X-Teacher-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(teacherIdHeader) || !Guid.TryParse(teacherIdHeader, out var teacherId))
                {
                    return BadRequest(new { error = "Teacher ID is required in X-Teacher-Id header (temporary for testing)" });
                }

                var query = new GetTeacherClassesQuery { TeacherId = teacherId };
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassCommand command)
        {
            try
            {
                var teacherIdHeader = Request.Headers["X-Teacher-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(teacherIdHeader) || !Guid.TryParse(teacherIdHeader, out var teacherId))
                {
                    return BadRequest(new { error = "Teacher ID is required in X-Teacher-Id header (temporary for testing)" });
                }

                command.TeacherId = teacherId;

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