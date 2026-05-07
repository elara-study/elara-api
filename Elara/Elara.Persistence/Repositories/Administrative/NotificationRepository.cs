using Elara.Application.Contracts.persistence.Administrative;
using Elara.Domain.Entities.Administrative;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories.Administrative
{
    public class NotificationRepository : BaseRepository<Notification, int>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Notification>> GetByUserId(Guid userId, int page, int pageSize, CancellationToken ct = default)
        {
            var result = await _context.Notifications.AsNoTracking().Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n=>n.NotificationDate)
                .Skip((page-1)*pageSize).Take(pageSize)
                 .ToListAsync(ct);
            
            return result;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
        {
            var result = await _context.Notifications.Where(n => n.UserId == userId && !n.IsDeleted && !n.IsRead).CountAsync(ct);
            return result;
        }

        public async Task MarkAsReadAsync(Guid notificationPublicId, Guid userId, CancellationToken ct = default)
        {
            await _context.Notifications.Where(n => n.UserId == userId && n.PublicId == notificationPublicId)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true),ct);
        }

        public async Task MarkBatchAsReadAsync(List<Guid> notificationPublicIds, Guid userId, CancellationToken ct = default)
        {
           await _context.Notifications.Where(n=>notificationPublicIds.Contains(n.PublicId)&&n.UserId== userId)
                .ExecuteUpdateAsync(s=>s.SetProperty(n=>n.IsRead, true),ct);
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
        {
           await _context.Notifications.Where(n=>n.UserId==userId&&!n.IsRead)
                .ExecuteUpdateAsync(s=>s.SetProperty(n=>n.IsRead, true), ct);
        }

        public async Task AddRangeAsync(List<Notification> notifications, CancellationToken ct = default)
        {
            await _context.Notifications.AddRangeAsync(notifications, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
