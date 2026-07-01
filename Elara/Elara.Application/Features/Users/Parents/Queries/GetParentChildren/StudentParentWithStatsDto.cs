using Elara.Domain.Enums;

namespace Elara.Application.Features.Users.Parents.Queries.GetParentChildren
{
    public class StudentParentWithStatsDto
    {
        public int Id { get; set; }
        public Guid StudentId { get; set; }
        public StudentParentRelationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public GradeLevel GradeLevel { get; set; }
        public int TotalXP { get; set; }
        public int CurrentStreak { get; set; }
        public int CompletedLessonsCount { get; set; }
    }
}
