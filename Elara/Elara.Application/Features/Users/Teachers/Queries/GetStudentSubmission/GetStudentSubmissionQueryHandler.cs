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
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;
        private readonly IAsyncRepository<Problem, int> _problemRepository;
        private readonly IIdentityService _identityService;

        public GetStudentSubmissionQueryHandler(
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<StudentSubmissionAnswer, int> answerRepository,
            IAsyncRepository<Homework, int> homeworkRepository,
            IAsyncRepository<Problem, int> problemRepository,
            IIdentityService identityService)
        {
            _submissionRepository = submissionRepository;
            _answerRepository = answerRepository;
            _homeworkRepository = homeworkRepository;
            _problemRepository = problemRepository;
            _identityService = identityService;
        }

        public async Task<StudentSubmissionDetailDto> Handle(GetStudentSubmissionQuery request, CancellationToken cancellationToken)
        {
            var submission = await _submissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission with id {request.SubmissionId} not found.");
            }

            var homework = await _homeworkRepository.GetByIdAsync(submission.HomeworkId, cancellationToken);
            if (homework == null)
            {
                throw new KeyNotFoundException($"Homework with id {submission.HomeworkId} not found.");
            }

            var submissionAnswers = await _answerRepository.FindAsync(
                a => a.StudentSubmissionId == request.SubmissionId, cancellationToken);

            var allProblems = await _problemRepository.FindAsync(
                q => q.HomeworkId == submission.HomeworkId, cancellationToken);

            var studentName = await _identityService.GetUserNameByIdAsync(submission.StudentId) ?? $"Student {submission.StudentId.ToString()[..8]}";

            var dto = new StudentSubmissionDetailDto
            {
                SubmissionId = submission.Id,
                StudentName = studentName,
                Score = submission.Score > 0 || !string.IsNullOrWhiteSpace(submission.TeacherFeedback) ? submission.Score : null,
                MaxScore = homework.MaxScore
            };

            var problemsById = allProblems.ToDictionary(q => q.Id);

            foreach (var answer in submissionAnswers)
            {
                problemsById.TryGetValue(answer.ProblemId, out var problem);

                dto.Answers.Add(new SubmissionAnswerDto
                {
                    ProblemId = answer.ProblemId,
                    ProblemText = problem?.Text ?? "Unknown Question",
                    StudentTextAnswer = answer.TextAnswer,
                    StudentImageUrl = answer.ImageUrl
                });
            }

            return dto;
        }
    }
}
