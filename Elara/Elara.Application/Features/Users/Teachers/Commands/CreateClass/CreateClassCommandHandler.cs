using Elara.Application.Common;
using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.CreateClass
{
    public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, CreateClassResponse>
    {
        private readonly IAsyncRepository<Class,int> _classRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateClassCommandHandler(
            IAsyncRepository<Class, int> classRepository,
            ITeacherRepository teacherRepository,
            ICurrentUserService currentUserService)
        {
            _classRepository = classRepository;
            _teacherRepository = teacherRepository;
            _currentUserService = currentUserService;
        }

        public async Task<CreateClassResponse> Handle(CreateClassCommand request, CancellationToken cancellationToken)
        {
            var teacherId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var (teacher, roadmap) = await _teacherRepository.GetTeacherWithSubjectAndRoadmapAsync(
                teacherId, request.RoadmapName, cancellationToken);

            if (teacher == null)
            {
                throw new KeyNotFoundException($"Teacher with ID {teacherId} not found.");
            }

            if (teacher.Subject == null || teacher.SubjectId == null)
            {
                throw new InvalidOperationException("Teacher must be assigned to a subject before creating classes.");
            }

            if (!string.IsNullOrWhiteSpace(request.RoadmapName))
            {
                if (roadmap == null)
                {
                    throw new KeyNotFoundException(
                        $"Roadmap with name '{request.RoadmapName}' not found in your roadmaps. " +
                        $"Please check the roadmap name and try again.");
                }

                if (roadmap.TeacherId != teacherId)
                {
                    throw new UnauthorizedAccessException("You can only use your own roadmaps.");
                }

                if (roadmap.SubjectId != teacher.SubjectId)
                {
                    throw new InvalidOperationException(
                        $"The roadmap '{roadmap.Name}' is for a different subject. " +
                        $"Your subject: {teacher.Subject.Name}, Roadmap subject ID: {roadmap.SubjectId}");
                }

                if ((int)roadmap.Grade != request.Grade)
                {
                    throw new InvalidOperationException(
                        $"The roadmap '{roadmap.Name}' is for grade {(int)roadmap.Grade}, " +
                        $"but you're creating a class for grade {request.Grade}.");
                }
            }

            var joinCode = CodeGenerator.GenerateJoinCode();

            var newClass = new Class
            {
                ClassName = request.Name,
                Level = (GradeLevel)request.Grade,
                SubjectId = teacher.SubjectId.Value,
                TeacherId = teacherId,
                RoadmapId = roadmap?.Id,
                JoinCode = joinCode,
            };
            await _classRepository.AddAsync(newClass);

            return new CreateClassResponse
            {
                Id = newClass.Id,
                Name = newClass.ClassName,
                Grade = (int)newClass.Level,
                JoinCode = newClass.JoinCode,
                SubjectId = newClass.SubjectId,
                TeacherId = newClass.TeacherId
            };
        }

    }
}