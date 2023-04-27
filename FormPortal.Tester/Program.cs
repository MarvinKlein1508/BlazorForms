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
               builder.AddScoped<IDbProviderService>();
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


        }
    }




}