using Elara.Application.Contracts.Persistence.Users;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Users;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Elara.Persistence.Repositories.Users
{
    public class TeacherRepository:BaseRepository<Teacher,Guid>,ITeacherRepository
    {
        public TeacherRepository(AppDbContext context) : base(context){}
        public async Task<(Teacher teacher, Roadmap? roadmap)> GetTeacherWithSubjectAndRoadmapAsync(Guid teacherId,string? roadmapName = null,CancellationToken cancellationToken = default)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Subject).Include(t => t.Roadmaps)            
                .ThenInclude(r => r.Subject) 
                .FirstOrDefaultAsync(t => t.Id == teacherId && !t.IsDeleted, cancellationToken);

            if (teacher == null)
            {
                return (null!, null);
            }
            Roadmap? roadmap = null;
            if (!string.IsNullOrWhiteSpace(roadmapName))
            {
                var lowerRoadmapName = roadmapName.ToLower();
                roadmap = await _context.Roadmaps.Include(r => r.Subject)
                    .Include(r => r.Topics).FirstOrDefaultAsync(r => r.TeacherId == teacherId && r.Name.ToLower() == lowerRoadmapName && !r.IsDeleted, cancellationToken);
            }
            return (teacher, roadmap);
        }
    }
}
