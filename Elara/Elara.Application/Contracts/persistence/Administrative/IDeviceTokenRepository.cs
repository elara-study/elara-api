using Elara.Application.Contracts.Persistence;
using Elara.Domain.Entities.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Contracts.persistence.Administrative
{
    public interface IDeviceTokenRepository : IAsyncRepository<DeviceToken, int>
    {
        Task UpsertAsync(Guid userId, string token, CancellationToken ct = default);
        Task RemoveByTokenAsync(string token, CancellationToken ct = default);
        Task<List<string>> GetTokensByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
