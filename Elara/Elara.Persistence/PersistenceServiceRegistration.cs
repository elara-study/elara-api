using Elara.Application.Contracts.Persistence;
using Elara.Application.Contracts.Persistence.Administrative;
using Elara.Application.Contracts.Persistence.Chat;
using Elara.Application.Contracts.Persistence.Educational;
using Elara.Application.Contracts.Persistence.Users;
using Elara.Persistence.Repositories;
using Elara.Persistence.Repositories.Administrative;
using Elara.Persistence.Repositories.Chat;
using Elara.Persistence.Repositories.Educational;
using Elara.Persistence.Repositories.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elara.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register generic repository
            services.AddScoped(typeof(IAsyncRepository<,>), typeof(BaseRepository<,>));
            
            // Register specific repositories
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IRoadmapRepository, RoadmapRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();

            return services;
        }
    }
}

