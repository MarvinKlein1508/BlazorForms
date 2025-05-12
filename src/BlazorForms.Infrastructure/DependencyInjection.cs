using BlazorForms.Domain.Interfaces;
using BlazorForms.Infrastructure.Auth;
using BlazorForms.Infrastructure.Database;
using BlazorForms.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorForms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IUserRepository, UserRepository>();


        // Options
        services.AddOptions<LoginOptions>()
            .Bind(config.GetRequiredSection(LoginOptions.SectionName));
        return services;
    }
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>(_ => new DbInitializer(connectionString));
        return services;
    }
}
