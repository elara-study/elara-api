using System;
using System.ComponentModel.DataAnnotations;

namespace Elara.Domain.Entities.Administrative
{
    public class Announcement : BaseEntity
    {
        [Required]
        public Guid PublicId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        // Foreign Key
        public int ClassId { get; set; }

        // Navigation property
        public virtual Class Class { get; set; } = null!;
    }
}
