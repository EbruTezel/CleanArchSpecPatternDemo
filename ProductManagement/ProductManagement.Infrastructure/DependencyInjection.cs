using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Persistence;
using ProductManagement.Application.Persistence.Repositories;
using ProductManagement.Infrastructure.Persistence.DbContexts;
using ProductManagement.Infrastructure.Persistence.Repositories;
using ProductManagement.Infrastructure.Persistence;

namespace ProductManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext add
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repository add
            services.AddScoped<IProductRepository, ProductRepository>();

            // UnitOfWork add
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
