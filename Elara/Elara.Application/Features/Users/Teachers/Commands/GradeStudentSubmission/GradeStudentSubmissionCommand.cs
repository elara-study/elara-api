using MediatR;

namespace Elara.Application.Features.Users.Teachers.Commands.GradeStudentSubmission
{
    public class GradeStudentSubmissionCommand : IRequest<bool>
    {
        public GradeStudentSubmissionCommand(int submissionId, double awardedXp, string? feedback)
        {
            SubmissionId = submissionId;
            AwardedXp = awardedXp;
            Feedback = feedback;
        }

        public int SubmissionId { get; }
        public double AwardedXp { get; }
        public string? Feedback { get; }
    }
}
