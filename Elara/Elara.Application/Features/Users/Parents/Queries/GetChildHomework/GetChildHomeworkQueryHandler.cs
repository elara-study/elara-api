using Elara.Application.Common.Interfaces;
using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.JunctionTables;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Parents.Queries.GetChildHomework
{
    public class GetChildHomeworkQueryHandler : IRequestHandler<GetChildHomeworkQuery, ChildHomeworkDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly IAsyncRepository<StudentClass, int> _studentClassRepository;
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly IAsyncRepository<Subject, int> _subjectRepository;
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetChildHomeworkQueryHandler(
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            IAsyncRepository<StudentClass, int> studentClassRepository,
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            IAsyncRepository<Subject, int> subjectRepository,
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _studentClassRepository = studentClassRepository;
            _moduleRepository = moduleRepository;
            _homeworkRepository = homeworkRepository;
            _subjectRepository = subjectRepository;
            _submissionRepository = submissionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ChildHomeworkDto> Handle(GetChildHomeworkQuery request, CancellationToken cancellationToken)
        {
            var parentId = _currentUserService.UserId 
                ?? throw new UnauthorizedAccessException("Parent is not authenticated.");

            // Verify relationship
            var isParent = await _studentRepository.IsParentOfStudentAsync(parentId, request.ChildId, cancellationToken);
            if (!isParent)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this child's homework.");
            }

            var response = new ChildHomeworkDto();

            // Fetch enrolled classes
            var studentClasses = await _studentClassRepository.FindAsync(
                sc => sc.StudentId == request.ChildId && sc.IsActive && !sc.IsDeleted,
                cancellationToken);
            var classIds = studentClasses.Select(sc => sc.ClassId).ToList();

            if (!classIds.Any())
            {
                response.summary = new HomeworkSummaryDto { total = 0, submitted = 0, graded = 0 };
                return response;
            }

            var classes = await _classRepository.FindAsync(
                c => classIds.Contains(c.Id) && !c.IsDeleted,
                cancellationToken);
            var roadmapIds = classes.Select(c => c.RoadmapId).Where(id => id.HasValue).Select(id => id!.Value).ToList();

            if (!roadmapIds.Any())
            {
                response.summary = new HomeworkSummaryDto { total = 0, submitted = 0, graded = 0 };
                return response;
            }

            var modules = await _moduleRepository.FindAsync(
                m => roadmapIds.Contains(m.RoadmapId) && !m.IsDeleted,
                cancellationToken);
            var moduleIds = modules.Select(m => m.Id).ToList();

            if (!moduleIds.Any())
            {
                response.summary = new HomeworkSummaryDto { total = 0, submitted = 0, graded = 0 };
                return response;
            }

            var homeworks = await _homeworkRepository.FindAsync(
                h => moduleIds.Contains(h.ModuleId) && !h.IsDeleted,
                cancellationToken);

            if (!homeworks.Any())
            {
                response.summary = new HomeworkSummaryDto { total = 0, submitted = 0, graded = 0 };
                return response;
            }

            var submissions = await _submissionRepository.FindAsync(s => s.StudentId == request.ChildId && !s.IsDeleted, cancellationToken);

            var submissionsByHomework = submissions
                .GroupBy(s => s.HomeworkId)
                .ToDictionary(g => g.Key, g => g.First());

            var subjectIds = modules.Select(m => m.SubjectId).Distinct().ToList();
            var subjects = await _subjectRepository.FindAsync(
                s => subjectIds.Contains(s.Id) && !s.IsDeleted,
                cancellationToken);
            var subjectsMap = subjects.ToDictionary(s => s.Id);

            var classesByRoadmap = classes.Where(c => c.RoadmapId.HasValue).GroupBy(c => c.RoadmapId!.Value).ToDictionary(g => g.Key, g => g.First());
            var modulesMap = modules.ToDictionary(m => m.Id);

            int totalCount = homeworks.Count;
            int submittedCount = 0;
            int gradedCount = 0;

            foreach (var hw in homeworks)
            {
                modulesMap.TryGetValue(hw.ModuleId, out var module);
                var subjectName = module != null && subjectsMap.TryGetValue(module.SubjectId, out var sub) ? sub.Name : string.Empty;
                var className = module != null && classesByRoadmap.TryGetValue(module.RoadmapId, out var cls) ? cls.ClassName : string.Empty;

                submissionsByHomework.TryGetValue(hw.Id, out var subRecord);

                string status = "pending";
                string? scoreString = null;

                if (subRecord != null)
                {
                    bool isGraded = subRecord.Score > 0 || !string.IsNullOrWhiteSpace(subRecord.TeacherFeedback);
                    if (isGraded)
                    {
                        status = "graded";
                        scoreString = $"{subRecord.Score} / {hw.MaxScore}";
                        gradedCount++;
                        submittedCount++;
                    }
                    else
                    {
                        status = "submitted";
                        submittedCount++;
                    }
                }

                response.homework_list.Add(new HomeworkListItemDto
                {
                    id = hw.Id.ToString(),
                    module = module?.ModuleName ?? module?.Title ?? string.Empty,
                    title = hw.Title,
                    description = hw.Description ?? string.Empty,
                    subject = subjectName,
                    class_label = className,
                    status = status,
                    score = scoreString
                });
            }

            response.summary = new HomeworkSummaryDto
            {
                total = totalCount,
                submitted = submittedCount,
                graded = gradedCount
            };

            return response;
        }
    }
}
