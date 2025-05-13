using BlazorForms.Application.Auth;
using BlazorForms.Application.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BlazorForms.Web.Components.Pages.Account;

public partial class Login
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    public LoginInput Input { get; set; } = new LoginInput();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private string? _errorMessage;

    public async Task HandleLogin()
    {
        User? user = null;
        using var connection = await DbFactory.CreateConnectionAsync();
        if (loginOptions.Value.IsLocalLoginEnabled)
        {
            user = await userRepository.GetByUsernameAsync(Input.Username, connection);
            if (user is not null)
            {
                PasswordHasher<User> hasher = new();
                PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password, Input.Password + user.Salt);

                if (result is PasswordVerificationResult.Failed)
                {
                    user = null;
                }
            }
        }

        if (user is null && loginOptions.Value.IsLdapLoginEnabled)
        {
            LdapSettings ldapSettings = loginOptions.Value.LdapSettings;
            var result = LdapProvider.Authenticate(Input.Username, Input.Password, ldapSettings);

            if (result is not null && result.Guid is not null)
            {
                user = await userRepository.GetByActiveDirectoryGuid(result.Guid.Value, connection);

                if (user is null)
                {
                    user = new User()
                    {
                        ActiveDirectoryGuid = result.Guid.Value,
                        Username = Input.Username.ToUpper(),
                        Email = result.Attributes["mail"],
                        DisplayName = $"{result.Attributes["givenName"]} {result.Attributes["sn"]}",
                        Origin = "ad"
                    };

                    await userRepository.CreateAsync(user, connection);
                }
                else
                {
                    user.Email = result.Attributes["mail"];
                    user.DisplayName = $"{result.Attributes["givenName"]} {result.Attributes["sn"]}";
                    user.Username = Input.Username.ToUpper();

                    await userRepository.UpdateAsync(user, connection); 
                }
            }

        }

        if (user is not null)
        {
            var claims = new List<Claim>
            {
                new("userId", user.UserId.ToString()),
            };


            //foreach (var permission in user.Permissions)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, permission.Identifier));
            //}

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            Navigation.NavigateTo(ReturnUrl ?? "/");
        }
        else
        {
            _errorMessage = "Username oder Passwort ist falsch.";
        }
    }
}

public class LoginInput
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}