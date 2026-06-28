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
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateRoadmapCommandHandler(
            IRoadmapRepository roadmapRepository,
            ISubjectRepository subjectRepository,
            ITeacherRepository teacherRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _roadmapRepository = roadmapRepository;
            _subjectRepository = subjectRepository;
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

            var subject = await _subjectRepository.GetByNameAsync(request.Subject.ToString(), cancellationToken);
            if (subject == null)
                throw new KeyNotFoundException($"Subject '{request.Subject}' not found.");

            var roadmap = new Roadmap
            {
                Name = request.Name,
                Grade = (GradeLevel)request.Grade,
                SubjectId = subject.Id,
                TeacherId = teacherId,
            };

            var created = await _roadmapRepository.AddAsync(roadmap, cancellationToken);

            return _mapper.Map<CreateRoadmapResponse>(created);
        }
    }
}
