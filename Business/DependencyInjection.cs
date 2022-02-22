using Business.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Data;

namespace Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterBusinessDI(this IServiceCollection services)
        {
         
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<ICryptographyService, CryptographyService>();

            return services;
        }
    }
}
