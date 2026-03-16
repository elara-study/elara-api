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

        public CreateRoadmapCommandHandler(
            IRoadmapRepository roadmapRepository,
            ISubjectRepository subjectRepository,
            ITeacherRepository teacherRepository)
        {
            _roadmapRepository = roadmapRepository;
            _subjectRepository = subjectRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<CreateRoadmapResponse> Handle(CreateRoadmapCommand request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with ID {request.TeacherId} not found.");

            var subjectExists = await _subjectRepository.ExistsAsync(request.SubjectId, cancellationToken);
            if (!subjectExists)
                throw new KeyNotFoundException($"Subject with ID {request.SubjectId} not found.");

            var roadmap = new Roadmap
            {
                Name = request.Name,
                Grade = (GradeLevel)request.Grade,
                SubjectId = request.SubjectId,
                TeacherId = request.TeacherId,
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
