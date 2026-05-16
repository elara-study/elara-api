using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Features.ChatAnalysisReport.Queries.GetParentChildInsights;
using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetSingleChildInsights
{
    public class GetSingleChildInsightsQueryHandler
        : IRequestHandler<GetSingleChildInsightsQuery, SingleChildInsightDto>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetSingleChildInsightsQueryHandler(
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

        public async Task<SingleChildInsightDto> Handle(
            GetSingleChildInsightsQuery request,
            CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException();

            if (!await _studentRepository.IsParentOfStudentAsync(parentId, request.ChildId, cancellationToken))
                throw new KeyNotFoundException("Child not found.");

            var reports = await _chatRepository
                .GetReportsByStudentIdAsync(request.ChildId, cancellationToken);

            var childName = await _identityService.GetUserNameByIdAsync(request.ChildId);

            return new SingleChildInsightDto
            {
                ChildId = request.ChildId,
                ChildName = childName ?? string.Empty,
                Reports = reports.Select(r => new ParentReportItemDto
                {
                    ReportId = r.PublicId,
                    ConversationId = r.ConversationId,
                    Subject = r.Subject,
                    ReportText = r.ReportText,
                    AnalyzedMessageCount = r.AnalyzedMessageCount,
                    AnalyzedAt = r.UpdatedAt ?? r.CreatedAt
                }).ToList()
            };
        }
    }
}
