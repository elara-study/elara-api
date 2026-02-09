using Elara.Application.Common;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Features.Administrative.Classes.Commands.Create_Class;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Enums;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.Create_Class
{
    public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand>
    {
        private readonly IAsyncRepository<Class,int> _classRepository;
        private readonly ITeacherRepository _teacherRepository;

        public CreateClassCommandHandler(IAsyncRepository<Class, int> classRepository,ITeacherRepository teacherRepository)
        {
            _classRepository = classRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task Handle(CreateClassCommand request, CancellationToken cancellationToken)
        {
            var (teacher, roadmap) = await _teacherRepository.GetTeacherWithSubjectAndRoadmapAsync( request.TeacherId,request.RoadmapName, cancellationToken);

            if (teacher == null)
            {
                throw new KeyNotFoundException($"Teacher with ID {request.TeacherId} not found.");
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

                if (roadmap.TeacherId != request.TeacherId)
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
                RoadmapId = roadmap?.Id,             
                JoinCode = joinCode,
                ClassTeachers = new List<ClassTeacher>
                {
                    new ClassTeacher
                    {
                        TeacherId = request.TeacherId
                    }
                }
            };
            await _classRepository.AddAsync(newClass);
        }

    }
}