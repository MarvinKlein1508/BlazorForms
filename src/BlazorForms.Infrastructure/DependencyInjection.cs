using BlazorForms.Domain.Interfaces;
using BlazorForms.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorForms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>(_ => new DbInitializer(connectionString));
        return services;
    }
}
