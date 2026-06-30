using Asp.Versioning;
using Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview;
using Elara.Application.Features.Users.Teachers.Queries.GetModuleResources;
using Elara.Application.Responses;
using Elara.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/modules")]
    [Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
    public class ModulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ModulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{moduleId:guid}/resources")]
        [ProducesResponseType(typeof(BaseResponse<ModuleResourcesDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetModuleResources(Guid moduleId, CancellationToken cancellationToken)
        {
            var query = new GetModuleResourcesQuery { ModuleId = moduleId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new BaseResponse<ModuleResourcesDto>
            {
                Message = "Module resources retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("{moduleId:guid}/homework")]
        [ProducesResponseType(typeof(BaseResponse<HomeworkOverviewDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetHomeworkOverview(Guid moduleId, CancellationToken cancellationToken)
        {
            var query = new GetHomeworkOverviewQuery { ModuleId = moduleId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new BaseResponse<HomeworkOverviewDto>
            {
                Message = "Homework overview retrieved successfully.",
                Data = result
            });
        }
    }
}
