using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetConversationReport
{
    public class GetConversationReportQuery : IRequest<ConversationReportDto>
    {
        public Guid ConversationId { get; set; }
    }
}
