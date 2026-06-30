using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Administrative;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.AddInsight
{
    public class AddInsightCommandHandler : IRequestHandler<AddInsightCommand, AddInsightResponse>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IAsyncRepository<TeacherInsight, int> _teacherInsightRepository;
        private readonly ICurrentUserService _currentUserService;

        public AddInsightCommandHandler(
            ITeacherRepository teacherRepository,
            IAsyncRepository<TeacherInsight, int> teacherInsightRepository,
            ICurrentUserService currentUserService)
        {
            _teacherRepository = teacherRepository;
            _teacherInsightRepository = teacherInsightRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AddInsightResponse> Handle(AddInsightCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var teacher = await _teacherRepository.GetTeacherWithStudentsAsync(teacherId, cancellationToken)
                ?? throw new KeyNotFoundException("Teacher not found.");

            var hasRelationship = teacher.StudentTeachers.Any(st => st.StudentId == request.StudentId);
            if (!hasRelationship)
                throw new KeyNotFoundException("Student not found.");

            var insight = new TeacherInsight
            {
                StudentId = request.StudentId,
                TeacherId = teacherId,
                Content = request.Content
            };

            await _teacherInsightRepository.AddAsync(insight, cancellationToken);

            return new AddInsightResponse
            {
                Id = insight.PublicId,
                Content = insight.Content,
                CreatedAt = insight.CreatedAt
            };
        }
    }
}
