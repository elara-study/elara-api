using Elara.API;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Elara.Infrastructure")
    )
);

var app = builder.ConfigureServices();
app.ConfigurePipeline();

app.Run();
