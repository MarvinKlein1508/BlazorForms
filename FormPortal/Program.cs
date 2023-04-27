using BlazorDownloadFile;
using Dapper;
using DbController.TypeHandler;
using FluentValidation;
using FormPortal.Core.Services;
using FormPortal.Core.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Plk.Blazor.DragDrop;
using System.Reflection;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace FormPortal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            var config = builder.Configuration;
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor()
                .AddCircuitOptions(options =>
                {
                    options.DetailedErrors = true;
                });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, configureOptions =>
                {
                });

            builder.Services.AddBlazorDragDrop();
            builder.Services.AddBlazorDownloadFile();
            builder.Services.AddScoped<PermissionService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<FormService>();
            builder.Services.AddScoped<FormRowService>();
            builder.Services.AddScoped<FormColumnService>();
            builder.Services.AddScoped<FormElementService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<RuleService>();
            builder.Services.AddScoped<CalcRuleService>();
            builder.Services.AddScoped<FormEntryService>();
            builder.Services.AddScoped<FormStatusService>();
            builder.Services.AddScoped<FormEntryStatusChangeService>();
            builder.Services.AddHotKeys2();
            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Languages";
            });

            builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), false, true);

            builder.Services.AddOptions<EmailSettings>()
                .Bind(config.GetRequiredSection(EmailSettings.SectionName));
#if DEBUG

            builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.development.json"), true, true);
#endif

#if DEBUGKLEIN
            builder.Configuration.AddJsonFile(Path.Combine("D:\\", "formularportal.klein.json"), false, true);
#endif


            // FluentValidation
            builder.Services.AddValidatorsFromAssembly(Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FormPortal.Core.Validators.dll")));
            var app = builder.Build();
            using var serviceScope = app.Services.CreateScope();

            var services = serviceScope.ServiceProvider;
            

            await AppdatenService.InitAsync(builder.Configuration);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(AppdatenService.SupportedCultures[0].Name)
    .AddSupportedCultures(AppdatenService.SupportedCultureCodes)
    .AddSupportedUICultures(AppdatenService.SupportedCultureCodes);

            app.UseRequestLocalization(localizationOptions);

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }

        public static string GetVersion()
        {
            return Assembly.GetEntryAssembly()!.GetName()!.Version!.ToString(3);
        }
    }
}