using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Contracts.Identity;
using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetParentChildInsights
{
    public class GetParentChildInsightsQueryHandler
        : IRequestHandler<GetParentChildInsightsQuery, List<ParentChildInsightDto>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetParentChildInsightsQueryHandler(
            IChatRepository chatRepository,
            IStudentRepository studentRepository,
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _chatRepository = chatRepository;
            _studentRepository = studentRepository;
            _identityService = identityService;
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

            var reportsByStudent = reports.GroupBy(r => r.StudentId);

            var result = new List<ParentChildInsightDto>();

            foreach (var child in children)
            {
                var userName = await _identityService.GetUserNameByIdAsync(child.Id);

                var childReports = reportsByStudent
                    .FirstOrDefault(g => g.Key == child.Id)?
                    .Select(r => new ParentReportItemDto
                    {
                        ReportId = r.PublicId,
                        ConversationId = r.ConversationId,
                        Subject = r.Subject,
                        ReportText = r.ReportText,
                        AnalyzedMessageCount = r.AnalyzedMessageCount,
                        AnalyzedAt = r.UpdatedAt ?? r.CreatedAt
                    }).ToList() ?? [];

                result.Add(new ParentChildInsightDto
                {
                    ChildId = child.Id,
                    ChildName = userName ?? string.Empty,
                    Reports = childReports
                });
            }

            return result;
        }
    }
}
