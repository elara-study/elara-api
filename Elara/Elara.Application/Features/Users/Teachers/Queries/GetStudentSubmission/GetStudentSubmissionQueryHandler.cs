using Elara.Application.Contracts.Identity;
using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Queries.GetStudentSubmission
{
    public class GetStudentSubmissionQueryHandler : IRequestHandler<GetStudentSubmissionQuery, StudentSubmissionDetailDto>
    {
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IAsyncRepository<StudentSubmissionAnswer, int> _answerRepository;
        private readonly IAsyncRepository<Assignment, int> _assignmentRepository;
        private readonly IAsyncRepository<Question, int> _questionRepository;
        private readonly IIdentityService _identityService;

        public GetStudentSubmissionQueryHandler(
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<StudentSubmissionAnswer, int> answerRepository,
            IAsyncRepository<Assignment, int> assignmentRepository,
            IAsyncRepository<Question, int> questionRepository,
            IIdentityService identityService)
        {
            _submissionRepository = submissionRepository;
            _answerRepository = answerRepository;
            _assignmentRepository = assignmentRepository;
            _questionRepository = questionRepository;
            _identityService = identityService;
        }

        public async Task<StudentSubmissionDetailDto> Handle(GetStudentSubmissionQuery request, CancellationToken cancellationToken)
        {
            var submission = await _submissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission with id {request.SubmissionId} not found.");
            }

            var assignment = await _assignmentRepository.GetByIdAsync(submission.AssignmentId, cancellationToken);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with id {submission.AssignmentId} not found.");
            }

            var submissionAnswers = await _answerRepository.FindAsync(
                a => a.StudentSubmissionId == request.SubmissionId, cancellationToken);
          
            var assignmentId = submission.AssignmentId;
            var allQuestions = await _questionRepository.FindAsync(
                q => q.AssignmentId == submission.AssignmentId, cancellationToken);

            var studentName = await _identityService.GetUserNameByIdAsync(submission.StudentId) ?? $"Student {submission.StudentId.ToString()[..8]}";

            var dto = new StudentSubmissionDetailDto
            {
                SubmissionId = submission.Id,
                StudentName = studentName,
                Score = submission.Score > 0 || !string.IsNullOrWhiteSpace(submission.TeacherFeedback) ? submission.Score : null,
                MaxScore = assignment.MaxScore
            };

            var questionsById = allQuestions.ToDictionary(q => q.Id);

            foreach (var answer in submissionAnswers)
            {
                questionsById.TryGetValue(answer.QuestionId, out var question);
                
                dto.Answers.Add(new SubmissionAnswerDto
                {
                    ProblemId = answer.QuestionId,
                    ProblemText = question?.Text ?? "Unknown Question",
                    StudentTextAnswer = answer.TextAnswer,
                    StudentImageUrl = answer.ImageUrl
                });
            }

            return dto;
        }
    }
}
