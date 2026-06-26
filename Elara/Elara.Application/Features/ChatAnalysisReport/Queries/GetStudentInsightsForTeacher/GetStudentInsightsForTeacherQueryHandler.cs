using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Application.Contracts.Persistence.Users;
using MediatR;

namespace Elara.Application.Features.ChatAnalysisReport.Queries.GetStudentInsightsForTeacher
{
    public class GetStudentInsightsForTeacherQueryHandler
        : IRequestHandler<GetStudentInsightsForTeacherQuery, StudentInsightForTeacherDto>
    {
        private readonly IChatRepository _chatRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetStudentInsightsForTeacherQueryHandler(
            IChatRepository chatRepository,
            ITeacherRepository teacherRepository,
            IIdentityService identityService,
            ICurrentUserService currentUserService)
        {
            _chatRepository = chatRepository;
            _teacherRepository = teacherRepository;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<StudentInsightForTeacherDto> Handle(
            GetStudentInsightsForTeacherQuery request,
            CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException();

            var teacher = await _teacherRepository
                .GetTeacherWithStudentsAsync(teacherId, cancellationToken)
                ?? throw new KeyNotFoundException("Teacher not found.");

            var subjectName = teacher.Subject?.Name
                ?? throw new InvalidOperationException("Teacher has no assigned subject.");

            var hasRelationship = teacher.StudentTeachers
                .Any(st => st.StudentId == request.StudentId);

            if (!hasRelationship)
                throw new KeyNotFoundException("Student not found.");

            var reports = await _chatRepository
                .GetReportsByStudentIdAndTitleAsync(request.StudentId, subjectName, cancellationToken);

            var studentName = await _identityService.GetUserNameByIdAsync(request.StudentId);

            return new StudentInsightForTeacherDto
            {
                StudentId = request.StudentId,
                StudentName = studentName ?? string.Empty,
                Reports = reports.Select(r => new ReportItemDto
                {
                    ReportId = r.PublicId,
                    ConversationId = r.ConversationId,
                    ReportText = r.ReportText,
                    AnalyzedMessageCount = r.AnalyzedMessageCount,
                    AnalyzedAt = r.UpdatedAt ?? r.CreatedAt
                }).ToList()
            };
        }
    }
}
