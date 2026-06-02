using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.RateStudentSubmission
{
    public class RateStudentSubmissionCommand : IRequest<bool>
    {
        public RateStudentSubmissionCommand(int submissionId, double awardedXp)
        {
            SubmissionId = submissionId;
            AwardedXp = awardedXp;
        }

        public int SubmissionId { get; }
        public double AwardedXp { get; }
    }
}
