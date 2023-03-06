using DatabaseControllerProvider;
using FormPortal.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace FormPortal.Tester
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
           .ConfigureAppConfiguration(builder =>
           {
               builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
               builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
           })
           .ConfigureServices(builder =>
           {
               builder.AddScoped<DbProviderService>();
               builder.AddScoped<PermissionService>();
               builder.AddScoped<UserService>();
               builder.AddScoped<FormService>();
               builder.AddScoped<FormRowService>();
               builder.AddScoped<FormColumnService>();
               builder.AddScoped<FormElementService>();
               builder.AddScoped<AuthService>();
               builder.AddScoped<RuleService>();
               builder.AddScoped<CalcRuleService>();
               builder.AddScoped<FormEntryService>();
           })
           .Build();

            var configuration = ActivatorUtilities.GetServiceOrCreateInstance<IConfiguration>(host.Services);

            await AppdatenService.InitAsync(configuration);

            using MySqlController sqlController = new MySqlController(AppdatenService.ConnectionString);

            var formEntryService = ActivatorUtilities.GetServiceOrCreateInstance<FormEntryService>(host.Services);

            var entry = await formEntryService.GetAsync(4, sqlController);

        }
    }
}