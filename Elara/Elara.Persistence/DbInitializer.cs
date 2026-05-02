using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Elara.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Lessons.AnyAsync())
            {
                await Elara.Scripts.DataPopulator.Run(context);
            }
        }
    }
}
