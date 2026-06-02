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

        public GetHomeworkSubmissionsQueryHandler(
            IAsyncRepository<Assignment, int> assignmentRepository,
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<Question, int> questionRepository,
            IIdentityService identityService)
        {
            _assignmentRepository = assignmentRepository;
            _submissionRepository = submissionRepository;
            _questionRepository = questionRepository;
            _identityService = identityService;
        }

        public async Task<List<HomeworkSubmissionDto>> Handle(GetHomeworkSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with id {request.AssignmentId} not found.");
            }

            var assignmentSubmissions = _submissionRepository.AsQueryable()
                .Where(s => s.AssignmentId == request.AssignmentId).ToList();

            var totalProblems = _questionRepository.AsQueryable()
                .Count(q => q.AssignmentId == request.AssignmentId);

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
                    var submittedAnswers = sub.Answers?.Count ?? 0;
                    
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
