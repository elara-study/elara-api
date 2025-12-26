using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Educational
{
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public GradeLevel GradeLevel { get; set; }

        // Navigation Properties
        public virtual ICollection<Topic> Topics { get; set; } 
        public virtual ICollection<Teacher> Teachers { get; set; }

    }
}
