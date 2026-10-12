using BlazorForms.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.FluentUI.AspNetCore.Components;
using BlazorForms.Infrastructure;
using BlazorForms.Infrastructure.Database;
using BlazorForms.Domain;
using Microsoft.AspNetCore.Identity;
using BlazorForms.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddFluentUIComponents(config =>
{
    config.DefaultValues.For<FluentTextArea>().Set(p => p.Width, "100%");
    config.DefaultValues.For<FluentSwitch>().Set(p => p.Margin, "0");
    config.DefaultValues.For<FluentCheckbox>().Set(p => p.Margin, "0");
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(AuthConstants.TwoFactorCookieName);

builder.Services.AddDatabase(config.GetConnectionString("Default")!);
builder.Services.AddApplication(config);

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Languages";
});

var app = builder.Build();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
var result = await dbInitializer.InitializeAsync();

await Storage.InitAsync(config);

////
//// Create a test user
////
//var userRepository = app.Services.GetRequiredService<UserRepository>();
//var dbFactory = app.Services.GetRequiredService<IDbConnectionFactory>();
//using var connection = await dbFactory.CreateConnectionAsync();
//User user = new User
//{
//    AccountType = AccountType.Internal,
//    Username = "admin",
//    Password = "admin",
//    DisplayName = "Admin",
//    IsActive = true,
//    Email = "admin@localhost",
//    Salt = SaltGenerator.GenerateSaltBase64(),
//    Role = UserRole.Administrator,
//};

//PasswordHasher<User> hasher = new();
//string passwordHashed = hasher.HashPassword(user, user.Password + user.Salt);
//user.Password = passwordHashed;
//await userRepository.CreateAsync(user, connection);

////
//// End create test user
////

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var accountGroup = app.MapGroup("/account");

accountGroup.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/account/login");
});

//var localizationOptions = new RequestLocalizationOptions()
//    .SetDefaultCulture(Storage.SupportedCultures[0].Name)
//    .AddSupportedCultures(Storage.SupportedCultureCodes)
//    .AddSupportedUICultures(Storage.SupportedCultureCodes);

//app.UseRequestLocalization(localizationOptions);

app.Run();
