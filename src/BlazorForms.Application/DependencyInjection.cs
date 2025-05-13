using BlazorForms.Application.Auth;
using BlazorForms.Application.Database;
using BlazorForms.Application.Domain;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorForms.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        // Configure dapper
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddSingleton<UserRepository>();
        services.AddSingleton<RoleRepository>();

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
