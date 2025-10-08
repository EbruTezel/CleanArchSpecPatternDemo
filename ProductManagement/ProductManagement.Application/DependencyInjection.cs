using Microsoft.Extensions.DependencyInjection;

namespace ProductManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR registration
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        // AutoMapper registration
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        
        return services;
    }
}