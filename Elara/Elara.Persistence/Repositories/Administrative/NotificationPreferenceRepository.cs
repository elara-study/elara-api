using Elara.Application.Contracts.persistence.Administrative;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Enums;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories.Administrative
{
    public class NotificationPreferenceRepository : BaseRepository<NotificationPreference, int>, INotificationPreferenceRepository
    {
        public NotificationPreferenceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.NotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId, ct);
        }

        public async Task<List<Guid>> GetUsersWithPreferenceEnabledAsync(List<Guid> userIds, NotificationType type, CancellationToken ct = default)
        {
            var prefs = await _context.NotificationPreferences
                .AsNoTracking()
                .Where(p => userIds.Contains(p.UserId))
                .ToListAsync(ct);

            var disabledUserIds = prefs
                .Where(p => !IsPreferenceEnabled(p, type))
                .Select(p => p.UserId)
                .ToHashSet();

            return userIds.Where(id => !disabledUserIds.Contains(id)).ToList();
        }

        private static bool IsPreferenceEnabled(NotificationPreference pref, NotificationType type)
        {
            return type switch
            {
                NotificationType.Announcement => pref.GroupUpdates,
                NotificationType.NewLesson => pref.NewLessons,
                NotificationType.StreakReminder => pref.StreakReminders,
                NotificationType.HomeworkReminder => pref.HomeworkReminders,
                NotificationType.AiProgressReport => pref.AiProgressReports,
                _ => true
            };
        }
    }
}
