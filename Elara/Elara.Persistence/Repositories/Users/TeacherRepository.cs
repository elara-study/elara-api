using Elara.Application.Contracts.Persistence.Users;
using Elara.Application.Models.Users;
using Elara.Domain.Entities.Administrative;
using Elara.Domain.Entities.Educational;
using Elara.Domain.Entities.Submissions;
using Elara.Domain.Entities.Users;
using Elara.Domain.Entities.JunctionTables;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories.Users
{
    public class TeacherRepository : BaseRepository<Teacher, Guid>, ITeacherRepository
    {
        public TeacherRepository(AppDbContext context) : base(context) { }

        public async Task<(Teacher teacher, Roadmap? roadmap)> GetTeacherWithSubjectAndRoadmapAsync(
            Guid teacherId, string? roadmapName = null, CancellationToken cancellationToken = default)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Subject)
                .Include(t => t.Roadmaps)
                    .ThenInclude(r => r.Subject)
                .FirstOrDefaultAsync(t => t.Id == teacherId && !t.IsDeleted, cancellationToken);

            if (teacher == null)
                return (null!, null);

            Roadmap? roadmap = null;
            if (!string.IsNullOrWhiteSpace(roadmapName))
            {
                var lowerRoadmapName = roadmapName.ToLower();
                roadmap = await _context.Roadmaps
                    .Include(r => r.Subject)
                    .Include(r => r.Modules)
                    .FirstOrDefaultAsync(r => r.TeacherId == teacherId &&
                        r.Name.ToLower() == lowerRoadmapName && !r.IsDeleted, cancellationToken);
            }

            return (teacher, roadmap);
        }

        public async Task<Teacher?> GetTeacherWithStudentsAsync(Guid teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .Include(t => t.Subject)
                .Include(t => t.StudentTeachers)
                .FirstOrDefaultAsync(t => t.Id == teacherId && !t.IsDeleted, cancellationToken);
        }

        public async Task<TeacherProfileReadModel?> GetTeacherProfileAsync(Guid teacherId, CancellationToken cancellationToken = default)
        {
            var userProfile = await (
                from teacher in _context.Teachers
                join user in _context.Users on teacher.Id equals user.Id
                where teacher.Id == teacherId && !teacher.IsDeleted
                select new
                {
                    teacher.Id,
                    user.Username,
                    FullName = string.IsNullOrWhiteSpace(user.Name) ? user.Username : user.Name,
                    user.ImageUrl,
                    user.Email,
                    user.PhoneNumber
                }).FirstOrDefaultAsync(cancellationToken);

            if (userProfile == null)
                return null;

            var primarySubject = await _context.Teachers
                .Where(t => t.Id == teacherId)
                .Select(t => t.Subject != null ? t.Subject.Name : null)
                .FirstOrDefaultAsync(cancellationToken);

            var classSubjectNames = await _context.Classes
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.Subject.Name)
                .ToListAsync(cancellationToken);

            var roadmapSubjectNames = await _context.Roadmaps
                .Where(r => r.TeacherId == teacherId)
                .Select(r => r.Subject.Name)
                .ToListAsync(cancellationToken);

            var subjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(primarySubject))
                subjects.Add(primarySubject);

            foreach (var name in classSubjectNames.Concat(roadmapSubjectNames))
            {
                if (!string.IsNullOrWhiteSpace(name))
                    subjects.Add(name);
            }

            var totalStudents = await _context.Set<StudentClass>()
                .Where(sc => sc.IsActive && sc.Class.TeacherId == teacherId)
                .Select(sc => sc.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            var activeGroups = await _context.Classes
                .Where(c => c.TeacherId == teacherId && c.StudentClasses.Any(sc => sc.IsActive))
                .CountAsync(cancellationToken);

            var roadmapsCreated = await _context.Roadmaps
                .CountAsync(r => r.TeacherId == teacherId, cancellationToken);

            var homeworkPublished = await _context.Homework
                .Where(l => l.Module.Roadmap.TeacherId == teacherId && l.HomeworkVideos.Any())
                .CountAsync(cancellationToken);

            return new TeacherProfileReadModel
            {
                TeacherId = userProfile.Id,
                Username = userProfile.Username,
                FullName = userProfile.FullName,
                AvatarUrl = userProfile.ImageUrl,
                Email = userProfile.Email,
                Phone = userProfile.PhoneNumber,
                Subjects = subjects.OrderBy(s => s).ToList(),
                TotalStudents = totalStudents,
                ActiveGroups = activeGroups,
                RoadmapsCreated = roadmapsCreated,
                HomeworkPublished = homeworkPublished
            };
        }

        public async Task<List<Class>> GetClassesByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.Classes
                .Where(c => c.TeacherId == teacherId && !c.IsDeleted)
                .Include(c => c.StudentClasses.Where(sc => sc.IsActive))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Roadmap>> GetRoadmapsByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.Roadmaps
                .Where(r => r.TeacherId == teacherId && !r.IsDeleted)
                .Include(r => r.Subject)
                .Include(r => r.Modules)
                    .ThenInclude(t => t.Homeworks)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
        }

        public async Task<double> GetAvgCompletionByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default)
        {
            var query = _context.StudentSubmissions
                .Where(s => s.Homework.Module.Roadmap.TeacherId == teacherId &&
                            s.Homework.MaxScore > 0 &&
                            !s.IsDeleted);

            if (!await query.AnyAsync(cancellationToken)) return 0;

            return await query
                .AverageAsync(s => (s.Score / s.Homework.MaxScore) * 100, cancellationToken);
        }

        public async Task<List<StudentSubmission>> GetPendingSubmissionsAsync(Guid teacherId, int take, CancellationToken cancellationToken = default)
        {
            return await _context.StudentSubmissions
                .Where(s => s.Homework.Module.Roadmap.TeacherId == teacherId &&
                            s.Score == 0 &&
                            string.IsNullOrEmpty(s.TeacherFeedback) &&
                            !s.IsDeleted)
                .Include(s => s.Homework)
                .OrderBy(s => s.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<StudentSubmission>> GetRecentSubmissionsAsync(Guid teacherId, int take, CancellationToken cancellationToken = default)
        {
            return await _context.StudentSubmissions
                .Where(s => s.Homework.Module.Roadmap.TeacherId == teacherId && !s.IsDeleted)
                .OrderByDescending(s => s.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}
