using BlazorForms.Components;
using BlazorForms.Core.Services;
using BlazorForms.Core.Settings;
using Dapper;
using DbController.TypeHandler;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.FluentUI.AspNetCore.Components;

SqlMapper.AddTypeHandler(typeof(Guid), new GuidTypeHandler());
SqlMapper.RemoveTypeMap(typeof(Guid));
SqlMapper.RemoveTypeMap(typeof(Guid?));

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddRazorPages()
    .WithRazorPagesRoot("/Components/Pages");

builder.Services.AddFluentUIComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);


//builder.Services.AddBlazorDragDrop();
//builder.Services.AddBlazorDownloadFile();
builder.Services.AddSingleton<PermissionService>();
builder.Services.AddSingleton<UserService>();
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
builder.Services.AddScoped<SavedFilterService>();
builder.Services.AddScoped<NotificationService>();

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

var app = builder.Build();


await AppdatenService.InitAsync(builder.Configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
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
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
