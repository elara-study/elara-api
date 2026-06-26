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
    await Elara.Persistence.DbInitializer.RunMigrationsAsync(context);
    
    var moduleCount = await context.Modules.CountAsync();
    Console.WriteLine($"📚 Database Ready! Total Modules found: {moduleCount}");
    
    if (moduleCount > 0)
    {
        var firstModule = await context.Modules.OrderBy(m => m.Id).FirstOrDefaultAsync();
        Console.WriteLine($"💡 Hint: Use Module ID '{firstModule.Id}' for your first quiz test.");
    }
}

app.Run();
