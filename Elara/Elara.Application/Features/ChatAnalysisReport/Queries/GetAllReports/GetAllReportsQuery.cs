using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetAllReports
{
    public class GetAllReportsQuery : IRequest<List<GetReportsDto>>
    {
    }
}
