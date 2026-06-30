using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkSubmissions
{
    public class GetHomeworkSubmissionsQueryHandler : IRequestHandler<GetHomeworkSubmissionsQuery, List<HomeworkSubmissionDto>>
    {
        private readonly IAsyncRepository<Module, int> _moduleRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IAsyncRepository<Problem, int> _problemRepository;
        private readonly IIdentityService _identityService;
        private readonly IAsyncRepository<StudentSubmissionAnswer, int> _answerRepository;

        public GetHomeworkSubmissionsQueryHandler(
            IAsyncRepository<Module, int> moduleRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<Problem, int> problemRepository,
            IIdentityService identityService,
            IAsyncRepository<StudentSubmissionAnswer, int> answerRepository)
        {
            _moduleRepository = moduleRepository;
            _homeworkRepository = homeworkRepository;
            _submissionRepository = submissionRepository;
            _problemRepository = problemRepository;
            _identityService = identityService;
            _answerRepository = answerRepository;
        }

        public async Task<List<HomeworkSubmissionDto>> Handle(GetHomeworkSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var module = (await _moduleRepository.FindAsync(m => m.PublicId == request.ModuleId, cancellationToken)).FirstOrDefault();
            if (module == null)
            {
                throw new KeyNotFoundException($"Module not found.");
            }

            var homework = (await _homeworkRepository.FindAsync(
                h => h.ModuleId == module.Id, cancellationToken)).FirstOrDefault();

            if (homework == null)
            {
                return new List<HomeworkSubmissionDto>();
            }

            var assignmentSubmissions = await _submissionRepository.FindAsync(
                s => s.HomeworkId == homework.Id, cancellationToken);

            var totalProblems = await _problemRepository.CountAsync(
                 q => q.HomeworkId == homework.Id, cancellationToken);

            var isRatedRequest = request.Status.Equals("rated", StringComparison.OrdinalIgnoreCase);

            var result = new List<HomeworkSubmissionDto>();

            foreach (var sub in assignmentSubmissions)
            {
                var studentName = await _identityService.GetUserNameByIdAsync(sub.StudentId) ?? $"Student {sub.StudentId.ToString()[..8]}";
                var userImage = await _identityService.GetUserImageDataAsync(sub.StudentId);
                var avatarUrl = userImage?.ImageUrl;

                var isRated = sub.Score > 0 || !string.IsNullOrWhiteSpace(sub.TeacherFeedback);

                if (isRatedRequest && isRated)
                {
                    result.Add(new HomeworkSubmissionDto
                    {
                        SubmissionId = sub.Id,
                        StudentId = sub.StudentId,
                        StudentName = studentName,
                        AvatarUrl = avatarUrl,
                        Score = sub.Score,
                        MaxScore = homework.MaxScore
                    });
                }
                else if (!isRatedRequest && !isRated)
                {
                    var submittedAnswers = await _answerRepository.CountAsync(
    a => a.StudentSubmissionId == sub.Id, cancellationToken);

                    result.Add(new HomeworkSubmissionDto
                    {
                        SubmissionId = sub.Id,
                        StudentId = sub.StudentId,
                        StudentName = studentName,
                        AvatarUrl = avatarUrl,
                        SubmittedAnswers = submittedAnswers,
                        TotalProblems = totalProblems
                    });
                }
            }

            return result;
        }
    }
}
