using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Features.Users.Students.Queries.GetStudentGroups;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.JunctionTables;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Elara.Persistence.Repositories.Administrative
{
    public class ClassRepository:BaseRepository<Class,int>,IClassRepository
    {
        public ClassRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<List<Class>> GetClassesByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken)
        {
            return await _context.Classes
                .Where(c => c.TeacherId == teacherId && !c.IsDeleted)
                .Include(c => c.Subject)
                .Include(c => c.StudentClasses)
                .ToListAsync(cancellationToken);
        }

        public async Task<Class?> GetClassWithSubjectAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Classes
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }

        public async Task<Class?> GetClassByJoinCodeAsync(string joinCode, CancellationToken cancellationToken = default)
        {
            return await _context.Classes
                .FirstOrDefaultAsync(c => c.JoinCode == joinCode && !c.IsDeleted, cancellationToken);
        }

        public async Task<List<GetStudentGroupItem>> GetStudentGroupsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            var completionRate = await _context.Reports
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.GeneratedDate)
                .Select(r => r.CompletionRate)
                .FirstOrDefaultAsync(cancellationToken);

            return await _context.StudentClasses
                .Where(sc => sc.StudentId == studentId && sc.IsActive)
                .Select(sc => sc.Class)
                .Select(c => new GetStudentGroupItem
                {
                    Id = c.Id.ToString(),
                    Name = c.ClassName,
                    Subject = c.Subject.Name,
                    Grade = (int)c.Level,
                    Teacher = _context.Users
                        .Where(u => u.Id == c.TeacherId)
                        .Select(u => u.Name)
                        .FirstOrDefault() ?? string.Empty,
                    Stats = new GetStudentGroupStats
                    {
                        StudentsCount = c.StudentClasses.Count(s => s.IsActive),
                        Lessons = new GetStudentGroupLessons
                        {
                            Total = c.Roadmap == null
                                ? 0
                                : c.Roadmap.Topics.SelectMany(t => t.Lessons).Count(),
                            Completed = c.Roadmap == null
                                ? 0
                                : (int)Math.Round(
                                    c.Roadmap.Topics.SelectMany(t => t.Lessons).Count()
                                    * (completionRate / 100.0))
                        }
                    }
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetStudentsCountAsync(int classId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Where(sc => sc.ClassId == classId)
                .CountAsync(cancellationToken);
        }

        public async Task<bool> IsStudentJoinedClassAsync(Guid studentId, int classId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .AnyAsync(sc => sc.StudentId == studentId && sc.ClassId == classId && sc.IsActive, cancellationToken);
        }

        public async Task JoinClassAsync(Guid studentId, int classId, CancellationToken cancellationToken = default)
        {
            await _context.StudentClasses.AddAsync(new StudentClass
            {
                StudentId = studentId,
                ClassId = classId,
                IsActive = true,
                EnrolledDate = DateTime.UtcNow
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
