using Elara.Domain.Entities.JunctionTables;

namespace Elara.Domain.Entities.Educational
{
    public class Homework : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public int EstimatedDurationMinutes { get; set; } = 30;

        public int ModuleId { get; set; }

        public virtual Module Module { get; set; } = null!;

        public virtual ICollection<HomeworkVideo> HomeworkVideos { get; set; }
        public virtual ICollection<ProblemSet> ProblemSets { get; set; }
    }
}
