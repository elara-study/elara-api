using Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses;
using Elara.Domain.Entities.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Contracts.Persistence.Administrative
{
    public interface IClassRepository:IAsyncRepository<Class,int>
    {
        Task<Class?> GetClassWithSubjectAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Class>> GetClassesByTeacherIdAsync(Guid teacherId ,CancellationToken cancellationToken);
        Task<int> GetStudentsCountAsync(int classId, CancellationToken cancellationToken = default);
    }
}
