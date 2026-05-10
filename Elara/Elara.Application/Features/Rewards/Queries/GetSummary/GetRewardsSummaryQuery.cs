using Elara.Application.Features.Rewards.DTOs;
using MediatR;

namespace Elara.Application.Features.Rewards.Queries.GetSummary
{
    public class GetRewardsSummaryQuery : IRequest<RewardSummaryDto>
    {
    }
}
