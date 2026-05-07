using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Contracts.persistence.Administrative
{
    public interface INotificationPreferenceRepository : IAsyncRepository<NotificationPreference, int>
    {
        Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<Guid>> GetUsersWithPreferenceEnabledAsync(List<Guid> userIds, NotificationType type, CancellationToken ct = default);
    }
}
