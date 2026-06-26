using Elara.Domain.Entities.Educational;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elara.Domain.Entities.JunctionTables
{
    public class HomeworkVideo : BaseEntity
    {
        public int HomeworkId { get; set; }
        public int VideoId { get; set; }

        [ForeignKey(nameof(HomeworkId))]
        public virtual Homework Homework { get; set; } = null!;

        [ForeignKey(nameof(VideoId))]
        public virtual EducationalVideo Video { get; set; } = null!;
    }
}
