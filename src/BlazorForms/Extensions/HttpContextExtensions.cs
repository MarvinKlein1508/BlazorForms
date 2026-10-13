using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using BlazorForms.Domain;
using BlazorForms.Infrastructure;
using BlazorForms.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BlazorForms;

internal static class HttpContextExtensions
{
    public static async Task LoginAsync(this HttpContext httpContext, User user, bool rememberMe)
    {
        var claims = new List<Claim>
        {
            new("userId", user.UserId.ToString(CultureInfo.InvariantCulture)),
        };

        string role = user.Role switch
        {
            UserRole.Admin => BlazorFormConstants.Roles.Admin,
            UserRole.Editor => BlazorFormConstants.Roles.Editor,
            _ => BlazorFormConstants.Roles.User
        };

        claims.Add(new Claim(ClaimTypes.Role, role));

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe
        };

        await httpContext.SignOutAsync(AuthConstants.TwoFactorCookieName);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
    }

    public static Task SetTwoFactorCookieAsync(this HttpContext httpContext, User user)
    {
        var claims = new List<Claim>
        {
            new("userId", user.UserId.ToString(CultureInfo.InvariantCulture)),
        };

        var claimsIdentity = new ClaimsIdentity(claims, AuthConstants.TwoFactorCookieName);
        return httpContext.SignInAsync(AuthConstants.TwoFactorCookieName, new ClaimsPrincipal(claimsIdentity));
    }

    public static bool IsIPAddressWhitelisted(this HttpContext httpContext)
    {
        var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
        var ranges = new List<(uint Start, uint End)>();

        // Read ranges from appsettings: Security:InternalIpRanges (array of objects with MASK_FROM / MASK_TO)
        var section = configuration.GetSection("Security:InternalIpRanges");
        if (section.Exists())
        {
            foreach (var child in section.GetChildren())
            {
                var from = child["MASK_FROM"];
                var to = child["MASK_TO"];

                if (IPAddress.TryParse(from ?? string.Empty, out var startIp) &&
                    IPAddress.TryParse(to ?? string.Empty, out var endIp))
                {
                    if (startIp.IsIPv4MappedToIPv6)
                    {
                        startIp = startIp.MapToIPv4();
                    }

                    if (endIp.IsIPv4MappedToIPv6)
                    {
                        endIp = endIp.MapToIPv4();
                    }

                    if (startIp.AddressFamily == AddressFamily.InterNetwork && endIp.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ranges.Add((ToUint(startIp), ToUint(endIp)));
                    }
                }
            }
        }

        var ipAddress = httpContext.Connection.RemoteIpAddress;

        if (ipAddress is null)
        {
            return false;
        }

        // Normalize IPv4-mapped IPv6 addresses to IPv4
        if (ipAddress.IsIPv4MappedToIPv6)
        {
            ipAddress = ipAddress.MapToIPv4();
        }

        // Allow loopback addresses (IPv4 and IPv6)
        if (IPAddress.IsLoopback(ipAddress))
        {
            return true;
        }

        // Only check IPv4 ranges here.
        if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
        {
            return false;
        }

        var ipUint = ToUint(ipAddress);

        foreach (var (Start, End) in ranges)
        {
            if (ipUint >= Start && ipUint <= End)
            {
                return true;
            }
        }

        return false;

        static uint ToUint(IPAddress ip)
        {
            var b = ip.GetAddressBytes();
            // network byte order (big-endian)
            return ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
        }
    }
}
