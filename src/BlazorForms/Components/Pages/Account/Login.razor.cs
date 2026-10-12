using System.ComponentModel.DataAnnotations;
using BlazorForms.Domain;
using Microsoft.AspNetCore.Components;
using Localizer = BlazorForms.Languages.Components.Pages.Account.Login;

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
            _errorMessage = Localizer.ERROR_WRONG_CREDENTIALS;
        }
        else if (!user.IsActive)
        {
            _errorMessage = Localizer.ERROR_DISABLED_ACCOUNT;
        }
        else if (user.LockoutEnd is not null && user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            _errorMessage = string.Join(Localizer.ERROR_ACCOUNT_LOCKED, user.LockoutEnd.Value.LocalDateTime);
        }
        else if (user.TwoFactorEnabled && !HttpContext.IsIPAddressWhitelisted())
        {
            await HttpContext.SetTwoFactorCookieAsync(user);
            redirectManager.RedirectTo(
                "account/loginwith2fa",
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
