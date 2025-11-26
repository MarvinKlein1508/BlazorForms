using BlazorForms.Infrastructure.Auth;
using BlazorForms.Infrastructure.Database;
using BlazorForms.Infrastructure.Repositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorForms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        // Configure dapper
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddSingleton<UserRepository>();
        services.AddSingleton<FormStatusRepository>();
        services.AddSingleton<FormStatusDescriptionRepository>();
        services.AddSingleton<LanguageRepository>();
        services.AddScoped<AuthService>();

        // Options
        services.AddOptions<LoginOptions>()
            .Bind(config.GetRequiredSection(LoginOptions.SectionName));

        return services;
    }
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton(_ => new DbInitializer(connectionString));
        return services;
    }
}
