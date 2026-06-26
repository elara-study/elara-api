using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkSubmissions
{
    public class GetHomeworkSubmissionsQueryHandler : IRequestHandler<GetHomeworkSubmissionsQuery, List<HomeworkSubmissionDto>>
    {
        private readonly IAsyncRepository<ProblemSet, int> _problemSetRepository;
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IAsyncRepository<Question, int> _questionRepository;
        private readonly IIdentityService _identityService;
        private readonly IAsyncRepository<StudentSubmissionAnswer, int> _answerRepository;

        public GetHomeworkSubmissionsQueryHandler(
            IAsyncRepository<ProblemSet, int> problemSetRepository,
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<Question, int> questionRepository,
            IIdentityService identityService,
            IAsyncRepository<StudentSubmissionAnswer, int> answerRepository)
        {
            _problemSetRepository = problemSetRepository;
            _submissionRepository = submissionRepository;
            _questionRepository = questionRepository;
            _identityService = identityService;
            _answerRepository = answerRepository;
        }

        public async Task<List<HomeworkSubmissionDto>> Handle(GetHomeworkSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var problemSet = await _problemSetRepository.GetByIdAsync(request.ProblemSetId, cancellationToken);
            if (problemSet == null)
            {
                throw new KeyNotFoundException($"ProblemSet with id {request.ProblemSetId} not found.");
            }

            var assignmentSubmissions = await _submissionRepository.FindAsync(
                s => s.ProblemSetId == request.ProblemSetId, cancellationToken);

            var totalProblems = await _questionRepository.CountAsync(
                 q => q.ProblemSetId == request.ProblemSetId, cancellationToken);

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
                        MaxScore = problemSet.MaxScore
                    });
                }
                else if (!isRatedRequest && !isRated)
                {
                    // For unrated, count submitted answers
                    var submittedAnswers = await _answerRepository.CountAsync(
    a => a              .StudentSubmissionId == sub.Id, cancellationToken);

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
