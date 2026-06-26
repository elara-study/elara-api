using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Features.Users.Students.Queries.GetStudentGroups;
using Elara.Application.Features.Users.Students.Queries.GetStudentGroupModules;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.JunctionTables;
using Elara.Infrastructure.Data;
using Elara.Application.Features.Users.Teachers.Queries.GetGroupStudents;
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

        public async Task<Class?> GetClassWithSubjectByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
        {
            return await _context.Classes
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.PublicId == publicId && !c.IsDeleted, cancellationToken);
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

            return await _context.Classes
                .AsNoTracking()
                .Where(c => c.StudentClasses.Any(sc => sc.StudentId == studentId && sc.IsActive))
                .Select(c => new GetStudentGroupItem
                {
                    Id = c.PublicId,
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
                                : c.Roadmap.Modules.SelectMany(t => t.Homeworks).Count(),
                            Completed = c.Roadmap == null
                                ? 0
                                : (int)Math.Round(
                                    c.Roadmap.Modules.SelectMany(t => t.Homeworks).Count()
                                    * (completionRate / 100.0))
                        }
                    }
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<GetStudentGroupItem?> GetStudentGroupByPublicIdAsync(Guid studentId, Guid groupPublicId, CancellationToken cancellationToken = default)
        {
            var completionRate = await _context.Reports
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.GeneratedDate)
                .Select(r => r.CompletionRate)
                .FirstOrDefaultAsync(cancellationToken);

            var groupProjection = await _context.Classes
                .AsNoTracking()
                .Where(c => c.PublicId == groupPublicId)
                .Where(c => c.StudentClasses.Any(sc => sc.StudentId == studentId && sc.IsActive))
                .Select(c => new
                {
                    c.PublicId,
                    c.ClassName,
                    SubjectName = c.Subject.Name,
                    Grade = (int)c.Level,
                    c.TeacherId,
                    StudentsCount = c.StudentClasses.Count(sc => sc.IsActive),
                    TotalLessons = c.Roadmap == null
                        ? 0
                        : c.Roadmap.Modules.SelectMany(t => t.Homeworks).Count()
                })
                .FirstOrDefaultAsync(
                    cancellationToken);

            if (groupProjection == null)
            {
                return null;
            }

            var teacherName = await _context.Users
                .Where(u => u.Id == groupProjection.TeacherId)
                .Select(u => u.Name)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

            var totalLessons = groupProjection.TotalLessons;
            var completedLessons = totalLessons == 0
                ? 0
                : (int)Math.Round(totalLessons * (completionRate / 100.0));

            return new GetStudentGroupItem
            {
                Id = groupProjection.PublicId,
                Name = groupProjection.ClassName,
                Subject = groupProjection.SubjectName,
                Grade = groupProjection.Grade,
                Teacher = teacherName,
                Stats = new GetStudentGroupStats
                {
                    StudentsCount = groupProjection.StudentsCount,
                    Lessons = new GetStudentGroupLessons
                    {
                        Total = totalLessons,
                        Completed = completedLessons
                    }
                }
            };
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

            var classEntity = await _context.Classes.FindAsync(new object[] { classId }, cancellationToken);
            if (classEntity != null)
            {
                var teacherId = classEntity.TeacherId;
                var relationshipExists = await _context.StudentTeachers
                    .AnyAsync(st => st.StudentId == studentId && st.TeacherId == teacherId, cancellationToken);

                if (!relationshipExists)
                {
                    await _context.StudentTeachers.AddAsync(new StudentTeacher
                    {
                        StudentId = studentId,
                        TeacherId = teacherId
                    }, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsAndOwnedByTeacherAsync(Guid classPublicId, Guid teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.Classes
                .AsNoTracking()
                .AnyAsync(c => c.PublicId == classPublicId && c.TeacherId == teacherId && !c.IsDeleted, cancellationToken);
        }

        public async Task<int?> GetInternalIdByPublicIdAsync(Guid publicId, Guid teacherId, CancellationToken cancellationToken = default)
        {
            var result = await _context.Classes
                .AsNoTracking()
                .Where(c => c.PublicId == publicId && c.TeacherId == teacherId && !c.IsDeleted)
                .Select(c => c.Id)
                .Cast<int?>()
                .FirstOrDefaultAsync(cancellationToken);
            
            return result;
        }

        public async Task<GetStudentGroupModulesResponse?> GetStudentGroupModulesAsync(Guid studentId, Guid groupPublicId, CancellationToken cancellationToken = default)
        {
            return await _context.Classes
                .AsNoTracking()
                .Where(c => c.PublicId == groupPublicId && !c.IsDeleted)
                .Where(c => c.StudentClasses.Any(sc => sc.StudentId == studentId && sc.IsActive))
                .Select(c => new GetStudentGroupModulesResponse
                {
                    GroupId = c.PublicId,
                    GroupName = c.ClassName,
                    Modules = c.Roadmap == null
                        ? new List<ModuleDto>()
                        : c.Roadmap.Modules.Select(m => new ModuleDto
                        {
                            Id = m.Id,
                            Title = m.Title,
                            Description = m.Description
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<GetGroupStudentsResponse>> GetStudentsInClassAsync(Guid classPublicId, Guid teacherId, CancellationToken cancellationToken = default)
        {
            var query = from sc in _context.StudentClasses
                        join c in _context.Classes on sc.ClassId equals c.Id
                        join u in _context.Users on sc.StudentId equals u.Id
                        where c.PublicId == classPublicId && c.TeacherId == teacherId && !c.IsDeleted && sc.IsActive
                        select new GetGroupStudentsResponse
                        {
                            Id = sc.StudentId,
                            Name = u.Name,
                            Username = u.Username,
                            ImageUrl = u.ImageUrl,
                            EnrolledDate = sc.EnrolledDate
                        };

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<Guid>> GetStudentIdsByClassIdAsync(int classId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Where(sc => sc.ClassId == classId && sc.IsActive)
                .Select(sc => sc.StudentId)
                .ToListAsync(cancellationToken);
        }

    }
}
