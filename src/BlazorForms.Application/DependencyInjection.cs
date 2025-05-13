using BlazorForms.Application.Database;
using BlazorForms.Application.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorForms.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<UserRepository>();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>(_ => new DbInitializer(connectionString));
        return services;
    }
}
