using BlazorForms.Application.Services;
using BlazorForms.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorForms.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IUserService, UserService>();
        return services;
    }
}
