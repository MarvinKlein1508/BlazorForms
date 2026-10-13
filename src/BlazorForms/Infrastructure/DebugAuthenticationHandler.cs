using System.Globalization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using BlazorForms.Domain;
using BlazorForms.Infrastructure.Database;
using BlazorForms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BlazorForms.Infrastructure;

public sealed class DebugAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IDbConnectionFactory dbFactory, UserRepository userRepository) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        using var connection = await dbFactory.CreateConnectionAsync();
        var user = await userRepository.GetAsync(1, connection);

        var claims = new List<Claim>
        {
            new("userId", user!.UserId.ToString(CultureInfo.InvariantCulture)),
        };

        string role = user.Role switch
        {
            UserRole.Admin => BlazorFormConstants.Roles.Admin,
            UserRole.Editor => BlazorFormConstants.Roles.Editor,
            _ => BlazorFormConstants.Roles.User
        };

        claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, "Debug");
        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
