using Elara.API;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Elara.Infrastructure")
    )
);

var app = builder.ConfigureServices();
app.ConfigurePipeline();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    
    Console.WriteLine("🔍 Checking database content...");
    await Elara.Persistence.DbInitializer.SeedAsync(context);
    
    var lessonCount = await context.Lessons.CountAsync();
    Console.WriteLine($"📚 Database Ready! Total Lessons found: {lessonCount}");
    
    if (lessonCount > 0)
    {
        var firstLesson = await context.Lessons.OrderBy(l => l.Id).FirstOrDefaultAsync();
        Console.WriteLine($"💡 Hint: Use Lesson ID '{firstLesson.Id}' for your first quiz test.");
    }
}

app.Run();
