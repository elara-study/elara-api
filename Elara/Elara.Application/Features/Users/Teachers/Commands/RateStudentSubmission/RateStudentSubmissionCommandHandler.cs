using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.RateStudentSubmission
{
    public class RateStudentSubmissionCommandHandler : IRequestHandler<RateStudentSubmissionCommand, bool>
    {
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IAsyncRepository<Assignment, int> _assignmentRepository;

        public RateStudentSubmissionCommandHandler(
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<Assignment, int> assignmentRepository)
        {
            _submissionRepository = submissionRepository;
            _assignmentRepository = assignmentRepository;
        }

        public async Task<bool> Handle(RateStudentSubmissionCommand request, CancellationToken cancellationToken)
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

            if (request.AwardedXp < 0 || request.AwardedXp > assignment.MaxScore)
            {
                throw new InvalidOperationException($"Awarded XP must be between 0 and {assignment.MaxScore}.");
            }

            submission.Score = request.AwardedXp;
            if (string.IsNullOrWhiteSpace(submission.TeacherFeedback))
            {
                submission.TeacherFeedback = "Graded by teacher.";
            }

            await _submissionRepository.UpdateAsync(submission, cancellationToken);

            return true;
        }
    }
}
