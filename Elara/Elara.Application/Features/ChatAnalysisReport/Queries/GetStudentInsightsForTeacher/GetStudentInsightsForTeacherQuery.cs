using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetStudentInsightsForTeacher
{
    public class GetStudentInsightsForTeacherQuery : IRequest<StudentInsightForTeacherDto>
    {
        public Guid StudentId { get; set; }
    }
}
