using CollabHub.Application.Interfaces.Auth;
using CollabHub.Application.Mapping;
using CollabHub.Application.Services;
using CollabHub.Infrastructure.Repositories.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DependecyInjection
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            return services;
        }
    }
}
