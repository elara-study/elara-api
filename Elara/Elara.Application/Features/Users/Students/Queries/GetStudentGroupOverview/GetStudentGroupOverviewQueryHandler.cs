using Elara.Application.Contracts.Persistence.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Students.Queries.GetStudentGroupOverview
{
    public class GetStudentGroupOverviewQueryHandler : IRequestHandler<GetStudentGroupOverviewQuery, GetStudentGroupOverviewResponse>
    {
        private readonly IClassRepository _classRepository;

        public GetStudentGroupOverviewQueryHandler(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task<GetStudentGroupOverviewResponse> Handle(GetStudentGroupOverviewQuery request, CancellationToken cancellationToken)
        {
            var group = await _classRepository.GetStudentGroupByPublicIdAsync(request.StudentId, request.GroupId, cancellationToken);

            if (group == null)
            {
                throw new KeyNotFoundException("Group not found or you don't have access.");
            }

            return new GetStudentGroupOverviewResponse
            {
                Group = new StudentGroupOverviewGroup
                {
                    Name = group.Name,
                    Subject = group.Subject,
                    Grade = group.Grade
                },
                Progress = new StudentGroupOverviewProgress
                {
                    CurrentLesson = group.Stats.Lessons.Completed,
                    TotalLessons = group.Stats.Lessons.Total
                }
            };
        }
    }
}
