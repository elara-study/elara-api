using Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses;
using Elara.Application.Features.Users.Students.Queries.GetStudentGroups;
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
        Task<Class?> GetClassWithSubjectByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);
        Task<Class?> GetClassByJoinCodeAsync(string joinCode, CancellationToken cancellationToken = default);
        Task<List<Class>> GetClassesByTeacherIdAsync(Guid teacherId ,CancellationToken cancellationToken);
        Task<List<GetStudentGroupItem>> GetStudentGroupsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<GetStudentGroupItem?> GetStudentGroupByPublicIdAsync(Guid studentId, Guid groupPublicId, CancellationToken cancellationToken = default);
        Task<bool> IsStudentJoinedClassAsync(Guid studentId, int classId, CancellationToken cancellationToken = default);
        Task JoinClassAsync(Guid studentId, int classId, CancellationToken cancellationToken = default);
        Task<int> GetStudentsCountAsync(int classId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAndOwnedByTeacherAsync(Guid classPublicId, Guid teacherId, CancellationToken cancellationToken = default);
        Task<int?> GetInternalIdByPublicIdAsync(Guid publicId, Guid teacherId, CancellationToken cancellationToken = default);
    }
}
