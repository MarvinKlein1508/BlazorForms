using System.ComponentModel.DataAnnotations;
using BlazorForms.Domain;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components.Pages.Account;

public partial class Login
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    public LoginInput Input { get; set; }

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private string? _errorMessage;

    protected override Task OnInitializedAsync()
    {
        Input ??= new LoginInput();

        //#if DEBUG
        //        Input.UseLdap = true;
        //#endif

        return base.OnInitializedAsync();
    }
    public async Task HandleLogin()
    {
        User? user;

        if (Input.UseLdap)
        {
            user = await loginService.LdapLoginAsync(Input.Username, Input.Password);
        }
        else
        {
            user = await loginService.LoginAsync(Input.Username, Input.Password);
        }

        if (user is null)
        {
            _errorMessage = "Username oder Passwort ist falsch.";
        }
        else if (!user.IsActive)
        {
            _errorMessage = "Ihr Konto ist deaktiviert. Bitte wenden Sie sich an den Administrator.";
        }
        else if (user.LockoutEnd is not null && user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            _errorMessage = $"Ihr Konto ist gesperrt bis {user.LockoutEnd.Value.LocalDateTime}.";
        }
        else if (user.TwoFactorEnabled && !HttpContext.IsIPAddressWhitelisted())
        {
            await HttpContext.SetTwoFactorCookieAsync(user);
            redirectManager.RedirectTo(
                "Account/LoginWith2fa",
                new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
        }
        else
        {
            await HttpContext.LoginAsync(user, Input.RememberMe);
            Navigation.NavigateTo(ReturnUrl ?? "/");
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
    public bool UseLdap { get; set; }
    public bool RememberMe { get; set; }
}
