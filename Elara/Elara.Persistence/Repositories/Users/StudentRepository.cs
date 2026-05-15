using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Users;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elara.Persistence.Repositories.Users
{
    public class StudentRepository : BaseRepository<Student, Guid>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Guid?> GetStudentIdByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var query = from u in _context.Users
                        join s in _context.Students on u.Id equals s.Id
                        where u.Username == username
                        select u.Id;

            var result = await query.Cast<Guid?>().FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<IReadOnlyList<Student>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Where(s => s.ParentId == parentId && !s.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Guid>> GetTeacherIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Elara.Domain.Entities.JunctionTables.StudentTeacher>()
                .Where(st => st.StudentId == studentId)
                .Select(st => st.TeacherId)
                .ToListAsync(cancellationToken);
        }
    }
}
