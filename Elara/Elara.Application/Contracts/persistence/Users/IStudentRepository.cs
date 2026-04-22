using Elara.Domain.Entities.Users;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Elara.Application.Contracts.Persistence.Users
{
    public interface IStudentRepository : IAsyncRepository<Student, Guid>
    {
        Task<Guid?> GetStudentIdByUsernameAsync(string username, CancellationToken cancellationToken = default);
    }
}
