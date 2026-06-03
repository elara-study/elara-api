using Elara.Application.Contracts.Identity;
using Elara.Infrastructure.Auth;
using Elara.Infrastructure.Data;
using Elara.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Elara.Application.Common.Interfaces;
using Elara.Infrastructure.Media;
using Elara.Infrastructure.Email;
using Elara.Infrastructure.Chat;
using Elara.Application.Features.Chat;
using Elara.Infrastructure.Quiz;
using Elara.Infrastructure.Notifications;
using Microsoft.Extensions.Options;

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
            services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();
            services.AddHttpClient<IOAuthTokenValidator, OAuthTokenValidator>();

            services.Configure<BrevoOptions>(configuration.GetSection(BrevoOptions.SectionName));
            services.AddScoped<IEmailService, EmailService>();

            services.Configure<GeminiSettings>(configuration.GetSection(GeminiSettings.SectionName));
            services.Configure<RagApiSettings>(configuration.GetSection(RagApiSettings.SectionName));
            services.Configure<ChatSettings>(configuration.GetSection(ChatSettings.SectionName));
            services.AddHttpClient<IGeminiService, GeminiService>();
            services.AddHttpClient<IRagService, RagService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IAchievementEvaluationService, Elara.Infrastructure.Rewards.AchievementEvaluationService>();

            // Firebase / FCM
            FcmInitializer.Initialize(configuration);
            services.AddSingleton<INotificationService, FcmNotificationService>();

            services.Configure<ElaraReportSettings>(
                configuration.GetSection(ElaraReportSettings.SectionName));
            services.AddSingleton<IChatAnalysisQueue>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<ElaraReportSettings>>().Value;
                return new ChatAnalysisQueue(settings.QueueCapacity);
            });
            services.AddHttpClient<IElaraReportService, ElaraReportService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });
            services.AddHostedService<ChatAnalysisScheduler>();
            services.AddHostedService<ChatAnalysisWorker>();

            return services;
        }
    }
}
