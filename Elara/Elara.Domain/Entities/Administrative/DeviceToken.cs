using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Administrative
{
    public class DeviceToken: BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        [MaxLength(512)]
        public string Token { get; set; } = string.Empty;
        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
    }
}
