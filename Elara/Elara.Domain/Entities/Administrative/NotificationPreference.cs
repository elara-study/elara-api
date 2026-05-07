using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Domain.Entities.Administrative
{
    public class NotificationPreference:BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        public bool GroupUpdates { get; set; } = true;
        public bool StreakReminders { get; set; } = true;
        public bool HomeworkReminders { get; set; } = true;
        public bool NewLessons { get; set; } = true;
        public bool AiProgressReports { get; set; } = true;
    }
}
