using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetHomeworkProblems
{
    public class GetHomeworkProblemsQueryHandler : IRequestHandler<GetHomeworkProblemsQuery, List<ParentHomeworkProblemDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IAsyncRepository<StudentClass, int> _studentClassRepository;
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly IAsyncRepository<Problem, int> _problemRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetHomeworkProblemsQueryHandler(
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            IAsyncRepository<StudentClass, int> studentClassRepository,
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            IAsyncRepository<Problem, int> problemRepository,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _studentClassRepository = studentClassRepository;
            _moduleRepository = moduleRepository;
            _homeworkRepository = homeworkRepository;
            _problemRepository = problemRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<ParentHomeworkProblemDto>> Handle(GetHomeworkProblemsQuery request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            var homework = await _homeworkRepository.GetByIdAsync(request.HomeworkId, cancellationToken);
            if (homework == null)
            {
                throw new KeyNotFoundException("Homework not found.");
            }

            var module = await _moduleRepository.GetByIdAsync(homework.ModuleId, cancellationToken);
            if (module == null)
            {
                throw new KeyNotFoundException("Module not found.");
            }

            var children = await _studentRepository.GetByParentIdAsync(parentId, cancellationToken);
            if (!children.Any())
            {
                throw new UnauthorizedAccessException("You are not authorized to view this homework's problems.");
            }

            var childIds = children.Select(c => c.Id).ToList();

            var classes = await _classRepository.FindAsync(
                c => c.RoadmapId == module.RoadmapId && !c.IsDeleted,
                cancellationToken);
            var classIds = classes.Select(c => c.Id).ToList();

            var enrollments = await _studentClassRepository.FindAsync(
                sc => childIds.Contains(sc.StudentId) && classIds.Contains(sc.ClassId) && sc.IsActive && !sc.IsDeleted,
                cancellationToken);

            if (!enrollments.Any())
            {
                throw new UnauthorizedAccessException("You are not authorized to view this homework's problems.");
            }

            var problems = await _problemRepository.FindAsync(
                p => p.HomeworkId == request.HomeworkId && !p.IsDeleted,
                cancellationToken);

            var result = problems
                .Select((p, idx) => new ParentHomeworkProblemDto
                {
                    problem_id = p.Id.ToString(),
                    label = $"PROBLEM {idx + 1}",
                    question_text = p.Text
                })
                .ToList();

            return result;
        }
    }
}
