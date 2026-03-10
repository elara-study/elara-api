using Elara.Application.Contracts.Identity;
using Elara.Infrastructure.Auth;
using Elara.Infrastructure.Data;
using Elara.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Elara.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IIdentityService, IdentityService>();

            return services;
        }
    }
}
