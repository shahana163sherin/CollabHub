using CollabHub.Application.Interfaces.Auth;
using CollabHub.Infrastructure.Repositories.EF;
using CollabHub.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Infrastructure.DependencyInjection
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddScoped<IHashPassword, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
