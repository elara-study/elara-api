using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.GradeStudentSubmission
{
    public class GradeStudentSubmissionCommandHandler : IRequestHandler<GradeStudentSubmissionCommand, bool>
    {
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IAsyncRepository<Homework, int> _homeworkRepository;

        public GradeStudentSubmissionCommandHandler(
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<Homework, int> homeworkRepository)
        {
            _submissionRepository = submissionRepository;
            _homeworkRepository = homeworkRepository;
        }

        public async Task<bool> Handle(GradeStudentSubmissionCommand request, CancellationToken cancellationToken)
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

            if (request.AwardedXp < 0 || request.AwardedXp > homework.MaxScore)
            {
                throw new InvalidOperationException($"Awarded XP must be between 0 and {homework.MaxScore}.");
            }

            submission.Score = request.AwardedXp;
            if (request.Feedback != null)
            {
                submission.TeacherFeedback = request.Feedback;
            }
            else if (string.IsNullOrWhiteSpace(submission.TeacherFeedback))
            {
                submission.TeacherFeedback = "Graded by teacher.";
            }

            await _submissionRepository.UpdateAsync(submission, cancellationToken);

            return true;
        }
    }
}
