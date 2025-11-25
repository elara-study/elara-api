using Elara.Domain.Entities.Users;
using Elara.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Educational
{
    public class Subject : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public GradeLevel GradeLevel { get; set; }

        // Navigation Properties
        public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    }
}
