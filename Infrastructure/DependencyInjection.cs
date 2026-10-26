using Infrastructure.Persistence;
using Infrastructure.Persistence.Seeding;
using Interview.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<InterviewDbContext>(options =>
            options.UseInMemoryDatabase("Default"));
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }

    public static async Task InitialiseInfrastructureAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<InterviewDbContext>();

        await context.Database.EnsureCreatedAsync(cancellationToken);

        if (!await context.Categories.AnyAsync(cancellationToken))
        {
            await context.Categories.AddRangeAsync(
                InitialDataSet.GetCategories(),
                cancellationToken);
        }

        if (!await context.Products.AnyAsync(cancellationToken))
        {
            await context.Products.AddRangeAsync(
                InitialDataSet.Get(),
                cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
