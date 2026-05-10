using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Users;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


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

        public async Task<Student?> GetStudentWithAchievementsAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return await _context.Students
                .Include(s => s.StudentAchievements)
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        }

        public async Task<IReadOnlyList<Student>> GetTopStudentsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.TotalXP)
                .ThenBy(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetStudentRankAsync(Guid studentId, int studentTotalXp, DateTime studentCreatedAt, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Where(s => s.IsActive && (s.TotalXP > studentTotalXp || (s.TotalXP == studentTotalXp && s.CreatedAt < studentCreatedAt)))
                .CountAsync(cancellationToken) + 1;
        }

        public async Task<Dictionary<Guid, string>> GetStudentNamesAsync(IEnumerable<Guid> studentIds, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Where(u => studentIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => string.IsNullOrWhiteSpace(u.Name) ? u.Username : u.Name, cancellationToken);
        }
    }
}
