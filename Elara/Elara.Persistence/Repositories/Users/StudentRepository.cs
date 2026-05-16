using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Models.Users;
using Elara.Domain.Entities.JunctionTables;
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

        public async Task<IReadOnlyList<Student>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .Where(sp => sp.ParentId == parentId)
                .Select(sp => sp.Student)
                .Where(s => !s.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Guid>> GetParentIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .Where(sp => sp.StudentId == studentId)
                .Select(sp => sp.ParentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsParentOfStudentAsync(Guid parentId, Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentParents
                .AnyAsync(sp => sp.ParentId == parentId && sp.StudentId == studentId, cancellationToken);
        }

        public async Task<IReadOnlyList<Guid>> GetTeacherIdsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Elara.Domain.Entities.JunctionTables.StudentTeacher>()
                .Where(st => st.StudentId == studentId)
                .Select(st => st.TeacherId)
                .ToListAsync(cancellationToken);
        }
            
        public async Task<Student?> GetStudentWithAchievementsAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return await _context.Students
                .Include(s => s.StudentAchievements)
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        }

        public async Task<StudentProfileReadModel?> GetStudentProfileAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            var studentProfile = await (
                from student in _context.Students
                join user in _context.Users on student.Id equals user.Id
                where student.Id == studentId && !student.IsDeleted
                select new StudentProfileReadModel
                {
                    StudentId = student.Id,
                    Username = user.Username,
                    FullName = string.IsNullOrWhiteSpace(user.Name) ? user.Username : user.Name,
                    AvatarUrl = user.ImageUrl,
                    GradeLevel = student.GradeLevel,
                    TotalXP = student.TotalXP,
                    CurrentStreak = student.CurrentStreak
                }).FirstOrDefaultAsync(cancellationToken);

            if (studentProfile == null)
            {
                return null;
            }

            studentProfile.Parents = await (
                from link in _context.StudentParents
                join parentEntity in _context.Parents on link.ParentId equals parentEntity.Id
                join parentUser in _context.Users on parentEntity.Id equals parentUser.Id
                where link.StudentId == studentId && !parentEntity.IsDeleted
                orderby parentUser.Name, parentUser.Username
                select new ParentProfileReadModel
                {
                    Id = parentEntity.Id,
                    FullName = string.IsNullOrWhiteSpace(parentUser.Name) ? parentUser.Username : parentUser.Name,
                    AvatarUrl = parentUser.ImageUrl
                }).ToListAsync(cancellationToken);

            studentProfile.RecentAchievements = await _context.Set<StudentAchievement>()
                .Where(sa => sa.StudentId == studentId)
                .OrderByDescending(sa => sa.EarnedAt)
                .Take(3)
                .Select(sa => new StudentAchievementReadModel
                {
                    Id = sa.AchievementId,
                    Title = sa.Achievement.Title
                })
                .ToListAsync(cancellationToken);

            return studentProfile;
        }

        public async Task<IReadOnlyList<Student>> GetTopStudentsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var validatedPage = Math.Max(1, page);
            var validatedPageSize = Math.Max(1, pageSize);

            return await _context.Students
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.TotalXP)
                .ThenBy(s => s.CreatedAt)
                .Skip((validatedPage - 1) * validatedPageSize)
                .Take(validatedPageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetStudentRankAsync(Guid studentId, int studentTotalXp, DateTime studentCreatedAt, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Where(s => s.IsActive
                     && s.Id != studentId
                     && (s.TotalXP > studentTotalXp
                     || (s.TotalXP == studentTotalXp 
                     && s.CreatedAt < studentCreatedAt)))
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
