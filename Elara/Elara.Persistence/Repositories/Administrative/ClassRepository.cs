using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Features.Users.Teachers.Queries.GetTeacherClasses;
using Elara.Domain.Entities.Administrative;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Elara.Persistence.Repositories.Administrative
{
    public class ClassRepository:BaseRepository<Class,int>,IClassRepository
    {
        public ClassRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<List<GetTeacherClassesResponse>> GetClassesByTeacherIdAsync( Guid teacherId,CancellationToken cancellationToken)
        {
            return await _context.Classes
                .Where(c => c.ClassTeachers.Any(ct => ct.TeacherId == teacherId))
                .Select(c => new GetTeacherClassesResponse
                {
                    Id = c.Id,
                    Name = c.ClassName,
                    Subject = c.Subject.Name,
                    Grade = (int)c.Level,
                    StudentsCount = c.StudentClasses.Count()
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<Class?> GetClassWithSubjectAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Classes
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }
        public async Task<int> GetStudentsCountAsync(int classId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Where(sc => sc.ClassId == classId)
                .CountAsync(cancellationToken);
        }
    }
}
