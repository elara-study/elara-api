using AutoMapper;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence.Educational;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.CreateRoadmap
{
    public class CreateRoadmapCommandHandler : IRequestHandler<CreateRoadmapCommand, CreateRoadmapResponse>
    {
        private readonly IRoadmapRepository _roadmapRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateRoadmapCommandHandler(
            IRoadmapRepository roadmapRepository,
            ITeacherRepository teacherRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _roadmapRepository = roadmapRepository;
            _teacherRepository = teacherRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<CreateRoadmapResponse> Handle(CreateRoadmapCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var teacher = await _teacherRepository.GetByIdAsync(teacherId, cancellationToken);
            if (teacher == null)
                throw new KeyNotFoundException($"Teacher with ID {teacherId} not found.");

            if (teacher.SubjectId == null)
                throw new InvalidOperationException("Teacher must be assigned to a subject before creating roadmaps.");

            var roadmap = new Roadmap
            {
                Name = request.Name,
                Grade = (GradeLevel)request.Grade,
                SubjectId = teacher.SubjectId.Value,
                TeacherId = teacherId,
            };

            var created = await _roadmapRepository.AddAsync(roadmap, cancellationToken);

            return _mapper.Map<CreateRoadmapResponse>(created);
        }
    }
}
