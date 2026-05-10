using Elara.Application.Features.Rewards.DTOs;
using MediatR;

namespace Elara.Application.Features.Rewards.Queries.GetBadges
{
    public class GetBadgesQuery : IRequest<List<BadgeDto>>
    {
    }
}
