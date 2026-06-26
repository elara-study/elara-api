using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Persistence
{
    public static class DbInitializer
    {
        public static async Task RunMigrationsAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();
        }
    }
}
