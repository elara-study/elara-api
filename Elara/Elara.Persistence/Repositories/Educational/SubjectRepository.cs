using Elara.Application.Contracts.Persistence.Educational;
using Elara.Domain.Entities.Educational;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence.Repositories.Educational
{
    public class SubjectRepository : BaseRepository<Subject, int>, ISubjectRepository
    {
        public SubjectRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Subject?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s => s.Name == name && !s.IsDeleted, cancellationToken);
        }
    }
}
