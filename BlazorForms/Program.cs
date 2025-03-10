using BlazorDownloadFile;
using BlazorForms.Components;
using BlazorForms.Core;
using BlazorForms.Core.Database;
using BlazorForms.Core.Models;
using BlazorForms.Core.Services;
using BlazorForms.Core.Settings;
using Dapper;
using DbController;
using DbController.MySql;
using DbController.TypeHandler;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Plk.Blazor.DragDrop;
using Spectre.Console;
using System.Reflection;
using Toolbelt.Blazor.Extensions.DependencyInjection;

SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHandler());
SqlMapper.RemoveTypeMap(typeof(Guid));
SqlMapper.RemoveTypeMap(typeof(Guid?));

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages().WithRazorPagesRoot("/Components/Pages");

builder.Services.AddServerSideBlazor()
             .AddCircuitOptions(options =>
             {
                 options.DetailedErrors = true;
             });


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddHostedService<NotificationService>();

builder.Services.AddBlazorDragDrop();
builder.Services.AddBlazorDownloadFile();

builder.Services.AddApplication();
builder.Services.AddDatabase(config.GetConnectionString("Default")!);

builder.Services.AddHotKeys2();
builder.Services.AddBlazorBootstrap();
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Languages";
});

builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), false, true);

builder.Services.AddOptions<EmailSettings>()
    .Bind(config.GetRequiredSection(EmailSettings.SectionName));

#if DEBUG
builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.development.json"), true, true);
builder.Configuration.AddUserSecrets(typeof(Program).Assembly);
#endif


// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BlazorForms.Core.dll")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();


app.UseRouting();
app.UseAntiforgery();


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorPages();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
var result = await dbInitializer.InitializeAsync();
MySqlController.Initialize(config);
MySqlController.InitializeTypeAttributeCache();
await Storage.InitAsync(config);

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(Storage.SupportedCultures[0].Name)
    .AddSupportedCultures(Storage.SupportedCultureCodes)
    .AddSupportedUICultures(Storage.SupportedCultureCodes);

app.UseRequestLocalization(localizationOptions);

if (args.Length > 0 && args[0] == "-setup")
{
    // Ask for the user's favorite fruit
    var exitMenu = false;

    while (!exitMenu)
    {
        int choice = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("Please select an option")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices([1, 2, 3])
                .UseConverter(x => x switch
                {
                    1 => "1. Create local admin account",
                    2 => "2. Exit setup",
                    _ => string.Empty
                }));

        string? connectionString = builder.Configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            AnsiConsole.MarkupLine($"[red]Could not find a connection string. Please provide a valid connection string for MySql in appsettings.json[/]");
            AnsiConsole.MarkupLine("Example: [grey]Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;[/]");
            continue;
        }

        if (choice is 1)
        {
            AnsiConsole.MarkupLine($"ConnectionString:\"[green]{connectionString}[/]\"");
            using (IDbController dbController = new MySqlController(connectionString))
            {
                User user = new()
                {
                    Origin = "local",
                    Salt = BlazorForms.Core.Extensions.StringExtensions.RandomString(10),
                    Username = AnsiConsole.Ask<string>("Please enter username:"),
                    DisplayName = AnsiConsole.Ask<string>("Please enter display name:"),
                    Email = AnsiConsole.Prompt(
                    new TextPrompt<string>("Please enter email:")
                                .ValidationErrorMessage("[red]Please enter a valid email![/]")
                                .Validate(BlazorForms.Core.Extensions.StringExtensions.IsEmail)
                    ),
                    Password = AnsiConsole.Ask<string>("Please enter password:")
                };

                PasswordHasher<User> hasher = new();
                string passwordHashed = hasher.HashPassword(user, user.Password + user.Salt);

                user.Password = passwordHashed;

                var permissions = await PermissionService.GetAllAsync(dbController);
                user.Permissions = permissions;

                var userService = app.Services.GetService<UserService>();

                await userService!.CreateAsync(user, dbController);

                AnsiConsole.MarkupLine("[green]User has been created successfully![/]");
            }
        }
        else if (choice is 2)
        {
            exitMenu = true;
        }
    }
}

app.Run();


public static partial class Program
{
    public static string GetVersion()
    {
        return Assembly.GetEntryAssembly()!.GetName()!.Version!.ToString(3);
    }
}