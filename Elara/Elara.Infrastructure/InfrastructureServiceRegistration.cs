using Elara.Application.Contracts.Identity;
using Elara.Infrastructure.Auth;
using Elara.Infrastructure.Data;
using Elara.Infrastructure.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Elara.Application.Common.Interfaces;
using Elara.Infrastructure.Media;

namespace Elara.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));

            services.Configure<CloudinaryOptions>(configuration.GetSection(CloudinaryOptions.SectionName));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IPendingTokenService, PendingTokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();

            return services;
        }
    }
}
