using Interview.Application.Interfaces;
using Interview.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Interview.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
