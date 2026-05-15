using Asp.Versioning;
using Elara.Application.Features.Rewards.DTOs;
using Elara.Application.Features.Rewards.Queries.GetBadges;
using Elara.Application.Features.Rewards.Queries.GetLeaderboard;
using Elara.Application.Features.Rewards.Queries.GetSummary;
using Elara.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class RewardsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RewardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<RewardSummaryDto>> GetSummary(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetRewardsSummaryQuery(), cancellationToken);
            return Ok(new BaseResponse<RewardSummaryDto>
            {
                Message = "Rewards summary retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("badges")]
        public async Task<ActionResult<List<BadgeDto>>> GetBadges(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetBadgesQuery(), cancellationToken);
            return Ok(new BaseResponse<List<BadgeDto>>
            {
                Message = "Badges retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("leaderboard")]
        public async Task<ActionResult<LeaderboardDto>> GetLeaderboard([FromQuery] string period = "allTime", [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetLeaderboardQuery
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new BaseResponse<LeaderboardDto>
            {
                Message = "Leaderboard retrieved successfully.",
                Data = result
            });
        }
    }
}
