using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetHomeworkSubmissions
{
    public class GetHomeworkSubmissionsQueryHandler : IRequestHandler<GetHomeworkSubmissionsQuery, List<HomeworkSubmissionDto>>
    {
        private readonly IAsyncRepository<Assignment, int> _assignmentRepository;
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IAsyncRepository<Question, int> _questionRepository;
        private readonly IIdentityService _identityService;
        private readonly IAsyncRepository<StudentSubmissionAnswer, int> _answerRepository;

        public GetHomeworkSubmissionsQueryHandler(
            IAsyncRepository<Assignment, int> assignmentRepository,
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<Question, int> questionRepository,
            IIdentityService identityService,
            IAsyncRepository<StudentSubmissionAnswer, int> answerRepository)
        {
            _assignmentRepository = assignmentRepository;
            _submissionRepository = submissionRepository;
            _questionRepository = questionRepository;
            _identityService = identityService;
            _answerRepository = answerRepository;
        }

        public async Task<List<HomeworkSubmissionDto>> Handle(GetHomeworkSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with id {request.AssignmentId} not found.");
            }

            var assignmentSubmissions = await _submissionRepository.FindAsync(
                s => s.AssignmentId == request.AssignmentId, cancellationToken);

            var totalProblems = await _questionRepository.CountAsync(
                 q => q.AssignmentId == request.AssignmentId, cancellationToken);

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
                        MaxScore = assignment.MaxScore
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
