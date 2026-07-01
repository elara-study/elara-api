using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetParentDashboard
{
    public class GetParentDashboardQuery : IRequest<ParentDashboardDto>
    {
        public Guid ParentId { get; set; }
    }
}