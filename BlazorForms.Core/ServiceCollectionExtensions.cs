using BlazorForms.Core.Database;
using BlazorForms.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorForms.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<PermissionService>();
        services.AddSingleton<UserService>();
        services.AddScoped<FormService>();
        services.AddScoped<FormRowService>();
        services.AddScoped<FormColumnService>();
        services.AddScoped<FormElementService>();
        services.AddScoped<AuthService>();
        services.AddScoped<RuleService>();
        services.AddScoped<CalcRuleService>();
        services.AddScoped<FormEntryService>();
        services.AddScoped<FormStatusService>();
        services.AddScoped<FormEntryStatusChangeService>();
        services.AddScoped<SavedFilterService>();
        services.AddScoped<NotificationService>();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>(_ => new DbInitializer(connectionString));
        return services;
    }
}
