using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReKlik.BLL.Services;
using ReKlik.BLL.Services.Contract;
using ReKlik.DAL.DBContext;
using ReKlik.DAL.Repositories;
using ReKlik.DAL.Repositories.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.IOC
{
    public static class Dependency
    {
        public static void InjectDependencys(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ReKlikDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("cadenaSQL"));
            }, ServiceLifetime.Scoped);

            // Repositorios
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();


            // Servicios
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();

            // Automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Automapper
            //services.AddAutoMapper(typeof(Startup));

        }
    }
}
