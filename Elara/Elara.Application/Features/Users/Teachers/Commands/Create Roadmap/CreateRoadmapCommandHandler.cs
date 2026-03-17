using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Educational;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.Create_Roadmap
{
    public class CreateRoadmapCommandHandler : IRequestHandler<CreateRoadmapCommand, CreateRoadmapResponse>
    {
        private readonly IRoadmapRepository _roadmapRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateRoadmapCommandHandler(
            IRoadmapRepository roadmapRepository,
            ISubjectRepository subjectRepository,
            ITeacherRepository teacherRepository,
            ICurrentUserService currentUserService)
        {
            _roadmapRepository = roadmapRepository;
            _subjectRepository = subjectRepository;
            _teacherRepository = teacherRepository;
            _currentUserService = currentUserService;
        }

        public async Task<CreateRoadmapResponse> Handle(CreateRoadmapCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var teacher = await _teacherRepository.GetByIdAsync(teacherId, cancellationToken);
            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with ID {teacherId} not found.");

            var subjectId = (int)request.Subject;
            var subjectExists = await _subjectRepository.ExistsAsync(subjectId, cancellationToken);
            if (!subjectExists)
                throw new KeyNotFoundException($"Subject with ID {subjectId} not found.");

            var roadmap = new Roadmap
            {
                Name = request.Name,
                Grade = (GradeLevel)request.Grade,
                SubjectId = subjectId,
                TeacherId = teacherId,
            };

            var created = await _roadmapRepository.AddAsync(roadmap, cancellationToken);

            return new CreateRoadmapResponse
            {
                Id = created.Id,
                Name = created.Name,
                Grade = (int)created.Grade,
                SubjectId = created.SubjectId,
                TeacherId = created.TeacherId,
                CreatedAt = created.CreatedAt,
            };
        }
    }
}
