using Elara.Application.Features.Rewards.DTOs;
using MediatR;

namespace Elara.Application.Features.Rewards.Queries.GetLeaderboard
{
    public class GetLeaderboardQuery : IRequest<LeaderboardDto>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
