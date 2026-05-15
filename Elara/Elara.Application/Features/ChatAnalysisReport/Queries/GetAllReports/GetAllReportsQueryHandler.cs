using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetAllReports
{
    public class GetAllReportsQueryHandler
        : IRequestHandler<GetAllReportsQuery, List<GetReportsDto>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAllReportsQueryHandler(
            IChatRepository chatRepository,
            ICurrentUserService currentUserService)
        {
            _chatRepository = chatRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<GetReportsDto>> Handle(
            GetAllReportsQuery request,
            CancellationToken cancellationToken)
        {
            var studentId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException();

            var reports = await _chatRepository
                .GetReportsByStudentIdAsync(studentId, cancellationToken);

            return reports.Select(r => new GetReportsDto
            {
                ReportId = r.PublicId,
                ConversationId = r.ConversationId,
                Subject = r.Subject,
                ReportText = r.ReportText,
                AnalyzedMessageCount = r.AnalyzedMessageCount,
                AnalyzedAt = r.UpdatedAt ?? r.CreatedAt
            }).ToList();
        }
    }
}
