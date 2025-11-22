using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MediatR;
using AutoMapper;
using System.Threading.Tasks;

namespace Elara.Application
{

    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            
            services.AddAutoMapper(e=>
            {
                e.AddMaps(Assembly.GetExecutingAssembly());
            });

            
            services.AddMediatR(m =>
            {
                m.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }



}
