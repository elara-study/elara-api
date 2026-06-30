using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Features.Users.Teachers.Commands.AddInsight;
using Elara.Domain.Entities.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.EditInsight
{
    public class EditInsightCommandHandler : IRequestHandler<EditInsightCommand, AddInsightResponse>
    {
        private readonly IAsyncRepository<TeacherInsight, int> _teacherInsightRepository;
        private readonly ICurrentUserService _currentUserService;

        public EditInsightCommandHandler(
            IAsyncRepository<TeacherInsight, int> teacherInsightRepository,
            ICurrentUserService currentUserService)
        {
            _teacherInsightRepository = teacherInsightRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AddInsightResponse> Handle(EditInsightCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var insights = await _teacherInsightRepository.FindAsync(
                t => t.PublicId == request.InsightId && !t.IsDeleted, cancellationToken);

            var insight = insights.FirstOrDefault();
            if (insight == null)
                throw new KeyNotFoundException("Insight not found.");

            if (insight.TeacherId != teacherId)
                throw new UnauthorizedAccessException("You do not have access to this insight.");

            insight.Content = request.Content;

            await _teacherInsightRepository.UpdateAsync(insight, cancellationToken);

            return new AddInsightResponse
            {
                Id = insight.PublicId,
                Content = insight.Content,
                CreatedAt = insight.CreatedAt
            };
        }
    }
}
