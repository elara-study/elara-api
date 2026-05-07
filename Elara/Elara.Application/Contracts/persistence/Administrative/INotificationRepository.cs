using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Contracts.persistence.Administrative
{
    public interface INotificationRepository : IAsyncRepository<Notification, int>
    {
        Task<List<Notification>> GetByUserId(Guid userid, int page, int pageSize, CancellationToken ct = default);
        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
        Task MarkAsReadAsync(Guid notificationPublicId, Guid userId, CancellationToken ct = default);
        Task MarkBatchAsReadAsync(List<Guid> notificationPublicIds, Guid userId, CancellationToken ct = default);
        Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
        Task AddRangeAsync(List<Notification> notifications, CancellationToken ct = default);
    }
}
