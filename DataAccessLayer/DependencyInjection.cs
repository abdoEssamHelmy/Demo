using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Data;

namespace DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterDataAccessLayer(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);
            services.AddDbContext<DBContext>(ServiceLifetime.Scoped);
            
           
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            return services;
        }
    }
}
