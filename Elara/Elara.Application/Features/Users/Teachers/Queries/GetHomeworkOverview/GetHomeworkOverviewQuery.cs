using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkOverview
{
    public class GetHomeworkOverviewQuery : IRequest<HomeworkOverviewDto>
    {
        public int TopicId { get; set; }
    }
}
