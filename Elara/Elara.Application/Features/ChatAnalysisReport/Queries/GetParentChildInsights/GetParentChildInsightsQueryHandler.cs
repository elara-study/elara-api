using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Application.Contracts.Persistence.Users;
using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetParentChildInsights
{
    public class GetParentChildInsightsQueryHandler
        : IRequestHandler<GetParentChildInsightsQuery, List<ParentChildInsightDto>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetParentChildInsightsQueryHandler(
            IChatRepository chatRepository,
            IStudentRepository studentRepository,
            ICurrentUserService currentUserService)
        {
            _chatRepository = chatRepository;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<ParentChildInsightDto>> Handle(
            GetParentChildInsightsQuery request,
            CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException();

            var children = await _studentRepository.GetByParentIdAsync(parentId, cancellationToken);

            if (children.Count == 0)
                return [];

            var childIds = children.Select(c => c.Id).ToList();
            var reports = await _chatRepository.GetReportsByStudentIdsAsync(childIds, cancellationToken);
            var names = await _studentRepository.GetStudentNamesAsync(childIds, cancellationToken);

            var reportsByStudent = reports.GroupBy(r => r.StudentId);

            return children.Select(child => new ParentChildInsightDto
            {
                ChildId = child.Id,
                ChildName = names.GetValueOrDefault(child.Id, string.Empty),
                Reports = (reportsByStudent
                    .FirstOrDefault(g => g.Key == child.Id)?
                    .Select(r => new ParentReportItemDto
                    {
                        ReportId = r.PublicId,
                        ConversationId = r.ConversationId,
                        Title = r.Title,
                        ReportText = r.ReportText,
                        AnalyzedMessageCount = r.AnalyzedMessageCount,
                        AnalyzedAt = r.UpdatedAt ?? r.CreatedAt
                    }).ToList()) ?? []
            }).ToList();
        }
    }
}
