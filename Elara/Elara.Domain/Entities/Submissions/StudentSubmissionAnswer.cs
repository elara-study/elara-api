using Elara.Domain.Entities.Educational;

namespace Elara.Domain.Entities.Submissions
{
    public class StudentSubmissionAnswer : BaseEntity
    {
        public string? TextAnswer { get; set; }
        public string? ImageUrl { get; set; }

        public bool? IsCorrect { get; set; }

        // Foreign Keys
        public int StudentSubmissionId { get; set; }
        public int QuestionId { get; set; }

        // Navigation Properties
        public virtual StudentSubmission Submission { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }
}
