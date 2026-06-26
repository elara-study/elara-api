using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.RateStudentSubmission
{
    public class RateStudentSubmissionCommandHandler : IRequestHandler<RateStudentSubmissionCommand, bool>
    {
        private readonly IAsyncRepository<StudentSubmission, int> _submissionRepository;
        private readonly IAsyncRepository<ProblemSet, int> _problemSetRepository;

        public RateStudentSubmissionCommandHandler(
            IAsyncRepository<StudentSubmission, int> submissionRepository,
            IAsyncRepository<ProblemSet, int> problemSetRepository)
        {
            _submissionRepository = submissionRepository;
            _problemSetRepository = problemSetRepository;
        }

        public async Task<bool> Handle(RateStudentSubmissionCommand request, CancellationToken cancellationToken)
        {
            var submission = await _submissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission with id {request.SubmissionId} not found.");
            }

            var problemSet = await _problemSetRepository.GetByIdAsync(submission.ProblemSetId, cancellationToken);
            if (problemSet == null)
            {
                throw new KeyNotFoundException($"ProblemSet with id {submission.ProblemSetId} not found.");
            }

            if (request.AwardedXp < 0 || request.AwardedXp > problemSet.MaxScore)
            {
                throw new InvalidOperationException($"Awarded XP must be between 0 and {problemSet.MaxScore}.");
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
