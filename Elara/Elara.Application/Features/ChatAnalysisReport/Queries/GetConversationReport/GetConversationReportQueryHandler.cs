using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetConversationReport
{
    public class GetConversationReportQueryHandler
        : IRequestHandler<GetConversationReportQuery, ConversationReportDto>
    {
        private readonly IChatRepository _chatRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetConversationReportQueryHandler(
            IChatRepository chatRepository,
            ICurrentUserService currentUserService)
        {
            _chatRepository = chatRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ConversationReportDto> Handle(
            GetConversationReportQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException();

            var report = await _chatRepository
                .GetReportByConversationIdAsync(request.ConversationId, cancellationToken)
                ?? throw new KeyNotFoundException("Report not found for this conversation.");

            if (!string.Equals(Convert.ToString(report.StudentId), userId.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException();
            }
            return new ConversationReportDto
            {
                ReportId = report.PublicId,
                Subject = report.Subject,
                ReportText = report.ReportText,
                AnalyzedMessageCount = report.AnalyzedMessageCount,
                AnalyzedAt = report.UpdatedAt ?? report.CreatedAt
            };
        }
    }
}
