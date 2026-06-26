namespace Elara.Domain.Entities.Educational
{
    public class Module : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? ModuleName { get; set; }

        public int SubjectId { get; set; }

        public int RoadmapId { get; set; }

        public virtual Subject Subject { get; set; } = null!;
        public virtual Roadmap Roadmap { get; set; }
        public virtual ICollection<Homework> Homeworks { get; set; }
        public virtual ICollection<ProblemSet> ProblemSets { get; set; }
        public virtual ICollection<ModuleResource> Resources { get; set; } = new List<ModuleResource>();
    }
}
